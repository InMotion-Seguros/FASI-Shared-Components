using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Domain;

public class Security
{
    #region Properties

    #region properties inherited from membership

    /// <summary>
    /// Gets the regular expression used to evaluate a password.//Obtiene la expresión regular utilizada para evaluar una contraseña.
    /// </summary>
    /// <value>Gets the regular expression used to evaluate a password.//Obtiene la expresión regular utilizada para evaluar una contraseña.</value>
    /// <returns>Gets the regular expression used to evaluate a password.//Obtiene la expresión regular utilizada para evaluar una contraseña.</returns>
    /// <remarks>The PasswordStrengthRegularExpression property gets the regular expression used to evaluate password complexity from the provider specified in the Provider property.</remarks>
    public string PasswordStrengthRegularExpression { get; set; }

    /// <summary>
    /// Gets the minimum length required for a password.//Obtiene la longitud mínima requerida para una contraseña.
    /// </summary>
    /// <value> Gets the minimum length required for a password.//Obtiene la longitud mínima requerida para una contraseña.</value>
    /// <returns> Gets the minimum length required for a password.//Obtiene la longitud mínima requerida para una contraseña.</returns>
    /// <remarks>The MinRequiredPasswordLength property gets the minimum number of characters that must be entered to create a valid password for the membership provider specified in the Provider property.//La propiedad MinRequiredPasswordLength obtiene la cantidad mínima de caracteres que deben ingresarse para crear una contraseña válida para el proveedor de membresía especificado en la propiedad del Proveedor.</remarks>
    public int MinRequiredPasswordLength { get; set; } = 6;

    /// <summary>
    /// Gets the maximum length required for a password.//Obtiene la longitud máxima requerida para una contraseña.
    /// </summary>
    /// <value> Gets the maximum length required for a password.//Obtiene la longitud máxima requerida para una contraseña.</value>
    /// <returns> Gets the maximum length required for a password.//Obtiene la longitud máxima requerida para una contraseña.</returns>
    /// <remarks>The MaxRequiredPasswordLength property gets the maximum number of characters that must be entered to create a valid password for the membership provider specified in the Provider property.//La propiedad MaxRequiredPasswordLength obtiene la cantidad máxima de caracteres que deben ingresarse para crear una contraseña válida para el proveedor de membresía especificado en la propiedad del Proveedor.</remarks>
    public int MaxRequiredPasswordLength { get; set; } = 20;

    /// <summary>
    /// Gets the minimum number of special characters that must be present in a valid password.//Obtiene la cantidad mínima de caracteres especiales que deben estar presentes en una contraseña válida.
    /// </summary>
    /// <value>Gets the minimum number of special characters that must be present in a valid password.//Obtiene la cantidad mínima de caracteres especiales que deben estar presentes en una contraseña válida.</value>
    /// <returns>Gets the minimum number of special characters that must be present in a valid password.//Obtiene la cantidad mínima de caracteres especiales que deben estar presentes en una contraseña válida.</returns>
    /// <remarks>The MinRequiredNonAlphanumericCharacters property returns the minimum number of special, non-alphanumeric characters that must be entered to create a valid password for the membership provider specified in the Provider property.//La propiedad MinRequiredNonAlphanumericCharacters devuelve el número mínimo de caracteres especiales no alfanuméricos que se deben ingresar para crear una contraseña válida para el proveedor de pertenencia especificado en la propiedad del Proveedor.</remarks>
    public int MinRequiredNonAlphanumericCharacters { get; set; } = 0;

    /// <summary>
    /// Gets a value indicating the format for storing passwords in the membership data store.//Obtiene un valor que indica el formato para almacenar contraseñas en el almacén de datos de membresía.
    /// </summary>
    /// <value>Enumerated type PasswordFormat</value>
    /// <returns>Enumerated type PasswordFormat</returns>
    /// <remarks></remarks>
    public Enumerations.EnumPasswordFormat PasswordFormat { get; set; } = Enumerations.EnumPasswordFormat.Encrypted;

    /// <summary>
    /// Gets the time window between which consecutive failed attempts to provide a valid password or password answer are tracked.//Obtiene la ventana de tiempo entre la que se realiza un seguimiento de los intentos fallidos consecutivos de proporcionar una contraseña válida o una respuesta de contraseña.
    /// </summary>
    /// <value>Gets the time window between which consecutive failed attempts to provide a valid password or password answer are tracked.//Obtiene la ventana de tiempo entre la que se realiza un seguimiento de los intentos fallidos consecutivos de proporcionar una contraseña válida o una respuesta de contraseña.</value>
    /// <returns>Gets the time window between which consecutive failed attempts to provide a valid password or password answer are tracked.//Obtiene la ventana de tiempo entre la que se realiza un seguimiento de los intentos fallidos consecutivos de proporcionar una contraseña válida o una respuesta de contraseña.</returns>
    /// <remarks>The PasswordAttemptWindow property works in conjunction with the MaxInvalidPasswordAttempts property to help guard against an unwanted source guessing the password or password answer of a membership user through repeated attempts. When a user attempts to log in with, change, or reset his or her password, only a certain number of consecutive attempts are allowed within a specified time window. The length of this time window is specified in the PasswordAttemptWindow property, which identifies the number of minutes allowed between invalid attempts.//La propiedad PasswordAttemptWindow funciona junto con la propiedad MaxInvalidPasswordAttempts para protegerse contra una fuente no deseada adivinando la contraseña o la contraseña de un usuario de membresía a través de intentos repetidos. Cuando un usuario intenta iniciar sesión, cambiar o restablecer su contraseña, solo se permite un cierto número de intentos consecutivos dentro de una ventana de tiempo específica. La duración de esta ventana de tiempo se especifica en la propiedad PasswordAttemptWindow, que identifica la cantidad de minutos permitidos entre intentos no válidos.</remarks>
    public int PasswordAttemptWindow { get; set; } = 0;

    /// <summary>
    /// Specifies the number of minutes after the last-activity date/time stamp for a user during which the user is considered online.
    /// </summary>
    /// <value>Specifies the number of minutes after the last-activity date/time stamp for a user during which the user is considered online.</value>
    /// <returns>Specifies the number of minutes after the last-activity date/time stamp for a user during which the user is considered online.</returns>
    /// <remarks>The UserIsOnlineTimeWindow property value is checked during the call to GetNumberOfUsersOnline. If the LastActivityDate for a user is greater than the current date and time minus the UserIsOnlineTimeWindow value in minutes, then the user is considered online. You can determine whether a membership user is considered online with the IsOnline property of the MembershipUser class.</remarks>
    public int UserIsOnlineTimeWindow { get; set; } = 0;

    /// <summary>
    /// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.//Obtiene la cantidad de intentos de contraseña inválida o de respuesta de contraseña permitida antes de que el usuario miembro se bloquee.
    /// </summary>
    /// <value>Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.//Obtiene la cantidad de intentos de contraseña inválida o de respuesta de contraseña permitida antes de que el usuario miembro se bloquee.</value>
    /// <returns>Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.//Obtiene la cantidad de intentos de contraseña inválida o de respuesta de contraseña permitida antes de que el usuario miembro se bloquee.</returns>
    /// <remarks>The MaxInvalidPasswordAttempts property works in conjunction with the PasswordAttemptWindow property to guard against an unwanted source using repeated attempts to guess the password or password answer of a membership user.//La propiedad MaxInvalidPasswordAttempts funciona junto con la propiedad PasswordAttemptWindow para protegerse de una fuente no deseada utilizando repetidos intentos de adivinar la contraseña o la contraseña de un usuario miembro.</remarks>
    public int MaxInvalidPasswordAttempts { get; set; } = 0;

    /// <summary>
    /// Validate rotation of the  passwords when is changed
    /// Validar la rotación de las contraseñas cuando se cambiar la misma
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public bool PasswordRotation { get; set; } = false;

    /// <summary>
    /// Number of days to expire the password of user
    /// Cantidad de días a expirar la contraseña del usuario
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public int PasswordExpirationCount { get; set; } = 90;

    /// <summary>
    /// Gets a value indicating whether the current membership provider is configured to allow users to retrieve their passwords.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>If EnablePasswordRetrieval is false, the underlying membership provider may throw a HttpException.</remarks>
    public bool EnablePasswordRetrieval { get; set; } = true;

    /// <summary>
    /// Gets a value indicating whether the default membership provider requires the user to answer a password question for password reset and retrieval.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>If RequiresQuestionAndAnswer is false, the underlying membership provider may throw a HttpException.</remarks>
    public bool RequiresQuestionAndAnswer { get; set; } = true;

    /// <summary>
    /// Define el estado de utilizar o no el ReCaptcha de Google
    /// Define the status of whether or not to use the Google ReCaptcha
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public bool RecaptchaEnable { get; set; } = false;

    #endregion properties inherited from membership

    #region Properties Portal Roles

    /// <summary>
    /// Property that defines the type of authentication system
    /// Propiedad que definen el tipo de autenticación del sistema
    /// </summary>
    /// <value>Enumerated type EnumSecurityMode</value>
    /// <returns>Enumerated type EnumSecurityMode</returns>
    /// <remarks></remarks>
    public Enumerations.EnumSecurityMode Mode { get; set; } = Enumerations.EnumSecurityMode.Database;

    /// <summary>
    /// Name of the anonymous role assigned to the user not logged.
    /// Nombre del rol anónimo asignado al usuario que no está logueado.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public string AnonymousRole { get; set; } = "Anonymous";

    /// <summary>
    /// Name of the default role assigned to the user.
    /// Nombre del rol por defecto asignado al usuario.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public string DefaultRole { get; set; } = "PortalUser";


    /// <summary>
    /// User name defined as administrator.
    /// Nombre del usuario definido como administrador.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public string AdministratorUser { get; set; } = "AdministratorUser";

    /// <summary>
    /// Name of the client role assigned to the user of same type.
    /// Nombre del rol de cliente asignado al usuario del mismo tipo.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public string ClientRole { get; set; } = "Client";

    /// <summary>
    /// Name of the producer role assigned to the user of same type.
    /// Nombre del rol de productor asignado al usuario del mismo tipo.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public string ProducerRole { get; set; } = "Producer";

    /// <summary>
    /// Nombre del role definido como administrador.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public string AdministratorRole { get; set; } = "Administrador";

    /// <summary>
    /// Nombre del role definido como Empleado.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public string EmployeeRole { get; set; } = "Empleado";

    #endregion Properties Portal Roles

    #region Security

    /// <summary>
    /// Define cuanto tiempo se tiene valido el token de usertoken
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public int TokenExpirationTime { get; set; } = 5;

    #endregion Security

    #endregion Properties
}