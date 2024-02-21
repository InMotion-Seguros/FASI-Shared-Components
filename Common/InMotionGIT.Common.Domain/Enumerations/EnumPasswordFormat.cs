
namespace InMotionGIT.Common.Domain.Enumerations;


public enum EnumPasswordFormat
{
    // 
    // Summary:
    // Passwords are not encrypted.
    Clear = 0,
    // 
    // Summary:
    // Passwords are encrypted one-way using the SHA1 hashing algorithm.
    Hashed = 1,
    // 
    // Summary:
    // Passwords are encrypted using the encryption settings determined by the machineKey
    // Element (ASP.NET Settings Schema) element configuration.
    Encrypted = 2
}