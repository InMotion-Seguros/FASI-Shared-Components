
namespace InMotionGIT.Common.Domain.Enumerations;


/// <summary>
/// Enumerated type that defines the authentication system
/// </summary>
/// <remarks></remarks>
[System.ComponentModel.DefaultValue(Database)]
public enum EnumSecurityMode
{
    Windows,
    Database,
    HeaderAuthentication,
    ActiveDirectory,
    Sesame,
    ActiveDirectoryAzure,
    ActiveDirectoryAzureSAML2
}