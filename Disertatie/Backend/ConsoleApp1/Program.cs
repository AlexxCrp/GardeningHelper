using System.Diagnostics;
using System.DirectoryServices;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace WindowsUserInfoConsole
{
    class Program
    {
        // P/Invoke declarations for native Windows APIs
        [DllImport("secur32.dll", CharSet = CharSet.Auto)]
        private static extern int GetUserNameEx(int nameFormat, System.Text.StringBuilder userName, ref uint userNameSize);

        // Name format constants
        private const int NameUnknown = 0;
        private const int NameFullyQualifiedDN = 1;
        private const int NameSamCompatible = 2;
        private const int NameDisplay = 3;
        private const int NameUniqueId = 6;
        private const int NameCanonical = 7;
        private const int NameUserPrincipal = 8;
        private const int NameCanonicalEx = 9;
        private const int NameServicePrincipal = 10;
        private const int NameDnsDomain = 12;

        // New method to handle both console and file logging
        private static void LogMessage(StreamWriter logFile, string message)
        {
            Console.WriteLine(message);
            logFile.WriteLine(message);
        }

        static void Main(string[] args)
        {
            // Create a log file in the user's Documents folder with timestamp
            string logFileName = $"WindowsUserInfo_{DateTime.Now:yyyyMMdd_HHmmss}.log";
            string logFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                logFileName
            );

            // Ensure the log directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

            using (StreamWriter logFile = new StreamWriter(logFilePath, true))
            {
                try
                {
                    LogMessage(logFile, "Windows User Info Console Application");
                    LogMessage(logFile, "Logging detailed user information...");
                    LogMessage(logFile, $"Log file: {logFilePath}");
                    LogMessage(logFile, "----------------------------------------");

                    // Redirect Console.Out to the log file for methods that might write directly to console
                    TextWriter originalConsoleOut = Console.Out;
                    Console.SetOut(logFile);

                    // Method 1: Environment Variables
                    LogMessage(logFile, "\n[Method 1: Environment Variables]");
                    LogMessage(logFile, $"Username: {Environment.UserName}");
                    LogMessage(logFile, $"Domain: {Environment.UserDomainName}");
                    LogMessage(logFile, $"Machine Name: {Environment.MachineName}");

                    // Method 2: Windows Identity
                    try
                    {
                        LogMessage(logFile, "\n[Method 2: Windows Identity]");
                        WindowsIdentity identity = WindowsIdentity.GetCurrent();
                        LogMessage(logFile, $"Identity Name: {identity.Name}");
                        LogMessage(logFile, $"Authentication Type: {identity.AuthenticationType}");
                        LogMessage(logFile, $"SID: {identity.User.Value}");
                        LogMessage(logFile, $"Is Admin: {IsUserAdmin()}");
                    }
                    catch (Exception ex)
                    {
                        LogMessage(logFile, $"Error getting Windows Identity: {ex.Message}");
                    }

                    // Method 3: GetUserNameEx API
                    LogMessage(logFile, "\n[Method 3: Windows Extended User Info]");
                    try
                    {
                        var nameFormats = new Dictionary<int, string>
                        {
                            { NameDisplay, "Display Name" },
                            { NameFullyQualifiedDN, "Distinguished Name" },
                            { NameSamCompatible, "SAM Compatible" },
                            { NameUserPrincipal, "User Principal" },
                            { NameCanonical, "Canonical Name" }
                        };

                        foreach (var format in nameFormats)
                        {
                            string name = GetUserNameExValue(format.Key);
                            if (!string.IsNullOrEmpty(name))
                            {
                                LogMessage(logFile, $"{format.Value}: {name}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMessage(logFile, $"Error getting extended user info: {ex.Message}");
                    }

                    // Method 4: WMI
                    LogMessage(logFile, "\n[Method 4: WMI User Info]");
                    try
                    {
                        using (ManagementObjectSearcher searcher =
                            new ManagementObjectSearcher($"SELECT * FROM Win32_UserAccount WHERE Name='{Environment.UserName}'"))
                        {
                            foreach (ManagementObject obj in searcher.Get())
                            {
                                LogMessage(logFile, $"WMI FullName: {obj["FullName"]}");
                                LogMessage(logFile, $"WMI Caption: {obj["Caption"]}");
                                LogMessage(logFile, $"WMI Description: {obj["Description"]}");
                                LogMessage(logFile, $"WMI Domain: {obj["Domain"]}");
                                LogMessage(logFile, $"WMI SID: {obj["SID"]}");
                                LogMessage(logFile, $"WMI Status: {obj["Status"]}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMessage(logFile, $"Error getting WMI user info: {ex.Message}");
                    }

                    // Method 5: Try LDAP connection to Active Directory 
                    LogMessage(logFile, "\n[Method 5: LDAP Query]");
                    try
                    {
                        string username = Environment.UserName;
                        string domainName = Environment.UserDomainName;

                        DirectoryEntry entry = new DirectoryEntry($"LDAP://{domainName}");
                        DirectorySearcher search = new DirectorySearcher(entry);
                        search.Filter = $"(sAMAccountName={username})";

                        search.PropertiesToLoad.Add("displayName");
                        search.PropertiesToLoad.Add("mail");
                        search.PropertiesToLoad.Add("department");
                        search.PropertiesToLoad.Add("title");
                        search.PropertiesToLoad.Add("telephoneNumber");

                        SearchResult result = search.FindOne();

                        if (result != null)
                        {
                            LogMessage(logFile, "Found user in Active Directory:");

                            if (result.Properties.Contains("displayName"))
                                LogMessage(logFile, $"Display Name: {result.Properties["displayName"][0]}");

                            if (result.Properties.Contains("mail"))
                                LogMessage(logFile, $"Email: {result.Properties["mail"][0]}");

                            if (result.Properties.Contains("department"))
                                LogMessage(logFile, $"Department: {result.Properties["department"][0]}");

                            if (result.Properties.Contains("title"))
                                LogMessage(logFile, $"Job Title: {result.Properties["title"][0]}");

                            if (result.Properties.Contains("telephoneNumber"))
                                LogMessage(logFile, $"Phone: {result.Properties["telephoneNumber"][0]}");
                        }
                        else
                        {
                            LogMessage(logFile, "User not found in Active Directory.");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMessage(logFile, $"Error querying Active Directory: {ex.Message}");
                    }

                    // Method 6: Windows Identity Groups
                    LogMessage(logFile, "\n[Method 6: Windows Identity Groups]");
                    try
                    {
                        GetUserGroups(logFile);
                    }
                    catch (Exception ex)
                    {
                        LogMessage(logFile, $"Error retrieving group memberships: {ex.Message}");
                    }

                    // Method 7: Try to get Azure AD user details using PowerShell
                    LogMessage(logFile, "\n[Method 7: Azure AD User Info via PowerShell]");
                    try
                    {
                        GetAzureADUserInfoViaPowerShell(logFile);
                    }
                    catch (Exception ex)
                    {
                        LogMessage(logFile, $"Error retrieving Azure AD info via PowerShell: {ex.Message}");
                    }

                    // Method 8: Get UPN and Azure AD indicators
                    LogMessage(logFile, "\n[Method 8: UPN and Azure AD Indicators]");
                    try
                    {
                        GetUserPrincipalNameAndAzureIndicators(logFile);
                    }
                    catch (Exception ex)
                    {
                        LogMessage(logFile, $"Error getting UPN info: {ex.Message}");
                    }

                    // Method 9: Registry information for Azure AD joined device
                    LogMessage(logFile, "\n[Method 9: Azure AD Device Registry Info]");
                    try
                    {
                        GetAzureADDeviceRegistryInfo(logFile);
                    }
                    catch (Exception ex)
                    {
                        LogMessage(logFile, $"Error getting Azure AD registry info: {ex.Message}");
                    }

                    // Restore original console output
                    Console.SetOut(originalConsoleOut);

                    LogMessage(logFile, "\nLog file generated successfully.");
                    Console.WriteLine($"\nDetailed log saved to: {logFilePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during logging: {ex.Message}");
                    logFile.WriteLine($"Error during logging: {ex.Message}");
                }
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static string GetUserNameExValue(int nameFormat)
        {
            uint size = 0;
            GetUserNameEx(nameFormat, null, ref size);

            if (size == 0) return null;

            System.Text.StringBuilder sb = new System.Text.StringBuilder((int)size);
            if (GetUserNameEx(nameFormat, sb, ref size) != 0)
            {
                return sb.ToString();
            }

            return null;
        }

        private static bool IsUserAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static void GetUserGroups(StreamWriter logFile)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            LogMessage(logFile, $"Groups for user {identity.Name}:");

            int groupCount = 0;
            if (identity.Groups != null)
            {
                foreach (IdentityReference group in identity.Groups)
                {
                    try
                    {
                        SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));

                        // Try to get the friendly name
                        string groupName;
                        try
                        {
                            groupName = group.Translate(typeof(NTAccount)).Value;
                        }
                        catch
                        {
                            groupName = sid.Value; // Use SID if name translation fails
                        }

                        LogMessage(logFile, $"  - {groupName} [{sid.Value}]");
                        groupCount++;
                    }
                    catch (Exception ex)
                    {
                        LogMessage(logFile, $"  - Error translating group: {ex.Message}");
                    }
                }
            }

            LogMessage(logFile, $"Total groups: {groupCount}");
        }

        private static void GetAzureADUserInfoViaPowerShell(StreamWriter logFile)
        {
            // Check if the Azure AD PowerShell module is available without prompting for login
            try
            {
                string script = @"
                    # Try to use cached credentials if available - this won't prompt for login if already connected
                    $currentUser = [Environment]::UserName
                    $currentDomain = [Environment]::UserDomainName
                    $userPrincipalName = $currentUser + '@' + $currentDomain.ToLower() + '.com'

                    Write-Output ""Detected UPN: $userPrincipalName""

                    # If Get-AzureADUser is available, use it
                    if (Get-Command Get-AzureADUser -ErrorAction SilentlyContinue) {
                        try {
                            Write-Output ""Trying to get Azure AD information with Get-AzureADUser...""
                            $result = Get-AzureADUser -ObjectId $userPrincipalName -ErrorAction SilentlyContinue
                            if ($result) {
                                $result | Format-List DisplayName, UserPrincipalName, Mail, JobTitle, Department
                            }
                        } catch {
                            Write-Output ""Could not get Azure AD info with Get-AzureADUser: $_""
                        }
                    } else {
                        Write-Output ""Get-AzureADUser command not available.""
                    }

                    # If MS Graph PowerShell is available, try that instead
                    if (Get-Command Get-MgUser -ErrorAction SilentlyContinue) {
                        try {
                            Write-Output ""Trying to get Azure AD information with Get-MgUser...""
                            $result = Get-MgUser -UserId $userPrincipalName -ErrorAction SilentlyContinue
                            if ($result) {
                                $result | Format-List DisplayName, UserPrincipalName, Mail, JobTitle, Department
                            }
                        } catch {
                            Write-Output ""Could not get Azure AD info with Get-MgUser: $_""
                        }
                    } else {
                        Write-Output ""Get-MgUser command not available.""
                    }

                    # Try to check if device is Azure AD joined without requiring authentication
                    Write-Output ""Running dsregcmd /status to check Azure AD join status...""
                    $dsregCmd = dsregcmd /status
                    $dsregOutput = $dsregCmd | Out-String

                    Write-Output ""Device Azure AD Join Status:""
                    if ($dsregOutput -match 'AzureAdJoined : YES') {
                        Write-Output ""This device is Azure AD joined.""
    
                        # Extract tenant information
                        if ($dsregOutput -match 'TenantId : (.+)') {
                            Write-Output ""Tenant ID: $($matches[1])""
                        }
    
                        if ($dsregOutput -match 'TenantName : (.+)') {
                            Write-Output ""Tenant Name: $($matches[1])""
                        }
    
                        # Look for user info
                        if ($dsregOutput -match 'UserEmail : (.+)') {
                            Write-Output ""User Email: $($matches[1])""
                        }
                    } else {
                        Write-Output ""This device is not Azure AD joined.""
                    }
                    ";

                // Create and configure the PowerShell process
                using (Process powershell = new Process())
                {
                    powershell.StartInfo.FileName = "powershell.exe";
                    powershell.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"";
                    powershell.StartInfo.UseShellExecute = false;
                    powershell.StartInfo.RedirectStandardOutput = true;
                    powershell.StartInfo.CreateNoWindow = true;

                    powershell.Start();
                    string output = powershell.StandardOutput.ReadToEnd();
                    powershell.WaitForExit();

                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        // Parse and display the output
                        LogMessage(logFile, output);
                    }
                    else
                    {
                        LogMessage(logFile, "No Azure AD information available through PowerShell.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage(logFile, $"Error running PowerShell: {ex.Message}");
            }
        }

        private static void GetUserPrincipalNameAndAzureIndicators(StreamWriter logFile)
        {
            // Try to get UPN (User Principal Name) which is typically used for Azure AD
            try
            {
                string userPrincipalName = GetUserNameExValue(NameUserPrincipal);
                if (!string.IsNullOrEmpty(userPrincipalName))
                {
                    LogMessage(logFile, $"User Principal Name (UPN): {userPrincipalName}");

                    // Check if the UPN looks like an email address, which is typical for Azure AD
                    if (userPrincipalName.Contains("@"))
                    {
                        LogMessage(logFile, "UPN format suggests Azure AD account (contains @)");

                        // Extract domain part of UPN
                        string domain = userPrincipalName.Split('@')[1];
                        LogMessage(logFile, $"Azure AD Domain: {domain}");

                        // Check if domain is a Microsoft online domain
                        if (domain.Contains("onmicrosoft.com") ||
                            domain.EndsWith(".microsoft.com") ||
                            domain.EndsWith(".windows.net"))
                        {
                            LogMessage(logFile, "Domain appears to be a Microsoft cloud domain");
                        }
                    }
                }
                else
                {
                    LogMessage(logFile, "Could not retrieve User Principal Name");
                }

                // Check for Azure AD related groups
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                bool hasAzureADGroups = false;

                if (identity.Groups != null)
                {
                    LogMessage(logFile, "\nChecking for Azure AD related groups:");
                    foreach (IdentityReference group in identity.Groups)
                    {
                        try
                        {
                            string groupName = group.Translate(typeof(NTAccount)).Value;
                            if (groupName.Contains("Azure") ||
                                groupName.Contains("Entra") ||
                                groupName.Contains("AAD") ||
                                groupName.Contains("Enterprise Apps") ||
                                groupName.Contains("Microsoft."))
                            {
                                LogMessage(logFile, $"  - Found Azure-related group: {groupName}");
                                hasAzureADGroups = true;
                            }
                        }
                        catch
                        {
                            // Skip groups that can't be translated
                        }
                    }

                    if (!hasAzureADGroups)
                    {
                        LogMessage(logFile, "  No obvious Azure AD related groups found");
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage(logFile, $"Error checking for Azure AD indicators: {ex.Message}");
            }
        }

        private static void GetAzureADDeviceRegistryInfo(StreamWriter logFile)
        {
            try
            {
                // Check registry locations that contain Azure AD join information
                // HKLM:\SYSTEM\CurrentControlSet\Control\CloudDomainJoin\JoinInfo
                string regOutput = RunCommand("reg", "query \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\CloudDomainJoin\\JoinInfo\" /s");

                if (!string.IsNullOrEmpty(regOutput) && !regOutput.Contains("ERROR:"))
                {
                    LogMessage(logFile, "Azure AD Join Information found in registry:");
                    LogMessage(logFile, regOutput);
                }
                else
                {
                    LogMessage(logFile, "No Azure AD Join information found in registry.");
                }

                // Check Work Account registry info
                string workAccountRegOutput = RunCommand("reg", "query \"HKLM\\SOFTWARE\\Microsoft\\WorkPlace\" /s");

                if (!string.IsNullOrEmpty(workAccountRegOutput) && !workAccountRegOutput.Contains("ERROR:"))
                {
                    LogMessage(logFile, "\nAzure AD Work Account Information:");
                    LogMessage(logFile, workAccountRegOutput);
                }

                // Run dsregcmd to get detailed Azure AD join status
                string dsregOutput = RunCommand("dsregcmd", "/status");

                if (!string.IsNullOrEmpty(dsregOutput))
                {
                    LogMessage(logFile, "\nDsregcmd Azure AD Join Status:");

                    // Parse the output for relevant information
                    var azureAdJoined = ExtractDsregValue(dsregOutput, "AzureAdJoined");
                    var tenantName = ExtractDsregValue(dsregOutput, "TenantName");
                    var tenantId = ExtractDsregValue(dsregOutput, "TenantId");
                    var userEmail = ExtractDsregValue(dsregOutput, "UserEmail");

                    LogMessage(logFile, $"Azure AD Joined: {azureAdJoined}");
                    if (azureAdJoined.Equals("YES", StringComparison.OrdinalIgnoreCase))
                    {
                        LogMessage(logFile, $"Tenant Name: {tenantName}");
                        LogMessage(logFile, $"Tenant ID: {tenantId}");
                        LogMessage(logFile, $"User Email: {userEmail}");
                    }
                }
                else
                {
                    LogMessage(logFile, "Could not retrieve Azure AD join status.");
                }
            }
            catch (Exception ex)
            {
                LogMessage(logFile, $"Error retrieving Azure AD registry information: {ex.Message}");
            }
        }

        private static string RunCommand(string command, string arguments)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = command;
                    process.StartInfo.Arguments = arguments;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    return output;
                }
            }
            catch (Exception ex)
            {
                return $"Error running {command}: {ex.Message}";
            }
        }

        private static string ExtractDsregValue(string dsregOutput, string key)
        {
            var match = Regex.Match(dsregOutput, $"{key} : (.+)");
            return match.Success ? match.Groups[1].Value.Trim() : "N/A";
        }
    }
}