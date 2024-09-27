using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using PeNet.Asn1;
using PeNet.Header.Pe;

namespace PeNet.Header.Authenticode

// References:
// a.	http://www.cs.auckland.ac.nz/~pgut001/pubs/authenticode.txt
{
    public class AuthenticodeInfo
    {
        private readonly PeFile _peFile;
        private readonly ContentInfo? _contentInfo;

        public string? SignerSerialNumber { get; }
        public byte[]? SignedHash { get; }
        public bool IsAuthenticodeValid { get; }
        public X509Certificate2? SigningCertificate { get; }

        public AuthenticodeInfo(PeFile peFile)
        {
            _peFile = peFile;
            
            _contentInfo = _peFile.WinCertificate == null
                ? null : new ContentInfo(_peFile.WinCertificate.BCertificate);

            SignerSerialNumber = GetSigningSerialNumber();
            SignedHash = GetSignedHash();
            IsAuthenticodeValid = VerifyHash() && VerifySignature();
            SigningCertificate = GetSigningCertificate();
        }

        private X509Certificate2? GetSigningCertificate()
        {
            if (_peFile.WinCertificate?.WCertificateType !=
                WinCertificateType.PkcsSignedData)
            {
                return null;
            }

            var pkcs7 = _peFile.WinCertificate.BCertificate.ToArray();

            // Workaround since the X509Certificate2 class does not return
            // the signing certificate in the PKCS7 byte array but crashes on Linux and macOS
            // when using .Net Core.
            // Under Windows with .Net Core the class works as intended.
            // See issue: https://github.com/dotnet/corefx/issues/25828
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                new X509Certificate2(pkcs7) : GetSigningCertificateNonWindows(pkcs7);
        }

        private X509Certificate2? GetSigningCertificateNonWindows(byte[] pkcs7)
        {
            // See https://github.com/dotnet/runtime/issues/15073#issuecomment-374787612
            var signedCms = new SignedCms();
            signedCms.Decode(pkcs7);
            var signerInfos = signedCms.SignerInfos.Cast<SignerInfo>().Where(si => string.Equals(si.Certificate?.SerialNumber, SignerSerialNumber, StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (signerInfos.Count == 1)
            {
                return signerInfos[0].Certificate;
            }
            var numberOfSignerInfos = signerInfos.Count == 0 ? "none" : signerInfos.Count.ToString();
            throw new CryptographicException($"Expected to find one certificate with serial number '{SignerSerialNumber}' but found {numberOfSignerInfos}.");
        }

        private bool VerifySignature()
        {
            var signedCms = new SignedCms();
            var bCert = _peFile.WinCertificate?.BCertificate.ToArray();
            if (bCert is null) return false;
            signedCms.Decode(bCert);

            try
            {
                // Throws an exception if the signature is invalid.
                signedCms.CheckSignature(true);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool VerifyHash()
        {
            if (SignedHash == null) return false;
            // 2.  Initialize a hash algorithm context.
            HashAlgorithm hashAlgorithm;
            switch (SignedHash.Length)
            {
                case 16:
                    hashAlgorithm = MD5.Create();
                    break;
                case 20:
                    hashAlgorithm = SHA1.Create();
                    break;
                case 32:
                    hashAlgorithm = SHA256.Create();
                    break;
                case 48:
                    hashAlgorithm = SHA384.Create();
                    break;
                case 64:
                    hashAlgorithm = SHA512.Create();
                    break;
                default:
                    return false;
            }
            var hash = ComputeAuthenticodeHashFromPeFile(hashAlgorithm);
            return hash != null && SignedHash.SequenceEqual(hash);
        }

        private byte[]? GetSignedHash()
        {
            if (_contentInfo?.Content is null)
                return null;

            if (_contentInfo?.ContentType != "1.2.840.113549.1.7.2") //1.2.840.113549.1.7.2 = OID for signedData
            {
                return null;
            }

            var sd = new SignedData(_contentInfo.Content);
            if (sd.ContentInfo.ContentType != "1.3.6.1.4.1.311.2.1.4") // 1.3.6.1.4.1.311.2.1.4 = OID for Microsoft Crypto
            {
                return null;
            }

            var spc = sd.ContentInfo.Content;
            if (spc is null) return null;
            var signedHash = (Asn1OctetString)spc.Nodes[0].Nodes[1].Nodes[1];
            return signedHash.Data;
        }

        private string? GetSigningSerialNumber()
        {
            var asn1 = _contentInfo?.Content;
            if (asn1 is null) return null;
            var x = (Asn1Integer)asn1.Nodes[0].Nodes[4].Nodes[0].Nodes[1].Nodes[1]; // ASN.1 Path to signer serial number: /1/0/4/0/1/1
#if NET48 || NETSTANDARD2_0
            return x.Value.ToHexString().Substring(2).ToUpper();
#else
            return x.Value.ToHexString()[2..].ToUpper();
#endif
        }

        public IEnumerable<byte>? ComputeAuthenticodeHashFromPeFile(HashAlgorithm hash)
        {
            var buff = _peFile.RawFile.ToArray();

            // 3.  Hash the image header from its base to immediately before the start of the checksum address, 
            // as specified in Optional Header Windows-Specific Fields.
            var offset = Convert.ToInt32(_peFile.ImageNtHeaders?.OptionalHeader.Offset) + 0x40;
            hash.TransformBlock(buff, 0, offset, new byte[offset], 0);

            // 4.  Skip over the checksum, which is a 4-byte field.
            offset += 0x4;

            // 6.  Get the Attribute Certificate Table address and size from the Certificate Table entry. 
            // For details, see section 5.7 of the PE/COFF specification.
            var certificateTable = _peFile.ImageNtHeaders?.OptionalHeader.DataDirectory[4];

            // 5.  Hash everything from the end of the checksum field to immediately before the start of the Certificate Table entry,
            // as specified in Optional Header Data Directories.
            var length = Convert.ToInt32(certificateTable?.Offset) - offset;
            hash.TransformBlock(buff, offset, length, new byte[length], 0);
            offset += length + 0x8;//end of Attribute Certificate Table addres

            // 7.  Exclude the Certificate Table entry from the calculation and 
            // hash everything from the end of the Certificate Table entry to the end of image header, 
            // including Section Table (headers). The Certificate Table entry is 8 bytes long, as specified in Optional Header Data Directories.
            length = Convert.ToInt32(_peFile.ImageNtHeaders?.OptionalHeader.SizeOfHeaders) - offset;// end optional header
            hash.TransformBlock(buff, offset, length, new byte[length], 0);

            // 8-13. Hash everything between end of header and certificate
            offset = Convert.ToInt32(_peFile.ImageNtHeaders?.OptionalHeader.SizeOfHeaders);

            if (_peFile.WinCertificate is not null)
            {
                length = Convert.ToInt32(_peFile.WinCertificate?.Offset) - offset;
                hash.TransformBlock(buff, offset, length, new byte[length], 0);

                // Move offset right beyond the Certificate Table
                offset += length + Convert.ToInt32(certificateTable?.Size);
            }

            // 14. Create a value called FILE_SIZE, which is not part of the signature. 
            // Set this value to the image’s file size, acquired from the underlying file system. 
            // If FILE_SIZE is greater than SUM_OF_BYTES_HASHED, the file contains extra data that must be added to the hash. 
            // This data begins at the SUM_OF_BYTES_HASHED file offset, and its length is:
            // (File Size) – ((Size of AttributeCertificateTable) + SUM_OF_BYTES_HASHED)
            // Note: The size of Attribute Certificate Table is specified 
            // in the second ULONG value in the Certificate Table entry (32 bit: offset 132, 64 bit: offset 148) in Optional Header Data Directories.
            // 14. Hash everything from the end of the certificate to the end of the file.
            var fileSize = buff.Length;
            if (fileSize > offset)
            {
                length = fileSize - offset;
                if (length != 0)
                {
                    hash.TransformBlock(buff, offset, length, new byte[length], 0);
                }
            }

            // 15. Finalize the hash algorithm context.
            hash.TransformFinalBlock(buff, 0, 0);
            return hash.Hash;
        }
    }
}