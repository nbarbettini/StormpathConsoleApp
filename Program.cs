using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stormpath.SDK;
using Stormpath.SDK.Client;
using Stormpath.SDK.Error;
using Stormpath.SDK.Application;
using Stormpath.SDK.Account;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Directory;
using Stormpath.SDK.CustomData;

namespace StormpathConsoleApp
{
    class Program
    {
        static IApplication g_app;


        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }


        static async Task MainAsync()
        {
            IClient client = Clients.Builder().SetApiKeyFilePath("C:\\Users\\Berk\\Documents\\Stormpath\\apiKey.properties").Build();
            g_app = await client.GetApplications().Where(x => x.Name == "My Application").SingleAsync();

            bool   isRunning = true;
            string email     = "";
            string password  = ""; ;
            string firstName = ""; ;
            string lastName  = ""; ;
            string age       = ""; ;

            try
            {
                while (isRunning)
                {
                    Console.WriteLine("(L)ogin, (A)dd account, (F)ind account, (S)how accounts, (D)elete account, (Q)uit?");
                    ConsoleKeyInfo input = Console.ReadKey();
                    Console.WriteLine("\n");

                    switch (input.KeyChar)
                    {
                        /////////////////////////////////////////////////////////////////////
                        case 'Q':
                        case 'q':
                            isRunning = false;
                            break;

                        /////////////////////////////////////////////////////////////////////
                        case 'A':
                        case 'a':
                            Console.Write("Email     : ");
                            email = Console.ReadLine();

                            Console.Write("Password  : ");
                            password = Console.ReadLine();

                            Console.Write("First name: ");
                            firstName = Console.ReadLine();

                            Console.Write("Last name : ");
                            lastName = Console.ReadLine();

                            Console.Write("Age       : ");
                            age = Console.ReadLine();

                            await CreateAccount(firstName, lastName, email, password, age);
                            break;

                        /////////////////////////////////////////////////////////////////////
                        case 'L':
                        case 'l':
                            Console.Write("Email     : ");
                            email = Console.ReadLine();

                            Console.Write("Password  : ");
                            password = Console.ReadLine();

                            await AuthAccount(email, password);
                            break;

                        /////////////////////////////////////////////////////////////////////
                        case 'D':
                        case 'd':
                            Console.Write("Email     : ");
                            email = Console.ReadLine();

                            await DeleteAccount(email);
                            break;

                        /////////////////////////////////////////////////////////////////////
                        case 'F':
                        case 'f':
                            Console.Write("Email     : ");
                            email = Console.ReadLine();

                            await FindAccount(email);
                            break;

                        /////////////////////////////////////////////////////////////////////
                        case 'S':
                        case 's':
                            await ShowAccounts();
                            break;
                    }//switch
                }//while
            }
            catch(ResourceException ex)
            {
                Console.WriteLine(">>> Error: " + ex.Message);
            }

            Console.ReadKey();
        }


        static async Task CreateAccount(string firstName, string lastName, string email, string password, string age)
        {
            IAccount accnt = await g_app.CreateAccountAsync(firstName, lastName, email, password);
            accnt.CustomData["Age"] = age;
            await accnt.SaveAsync();

            Console.WriteLine(">>> Account created: " + accnt.FullName);
        }


        static async Task AuthAccount(string email, string password)
        {
            IAuthenticationResult authResult = await g_app.AuthenticateAccountAsync(email, password);
            IAccount              accnt      = await authResult.GetAccountAsync();
            ICustomData           data       = await accnt.GetCustomDataAsync();

            Console.WriteLine(">>> Logged in as: " + accnt.FullName + ", age " + data["Age"]);
        }


        static async Task FindAccount(string email)
        {
            List<IAccount> accnts = await g_app.GetAccounts().Where(acct => acct.Email == email).ToListAsync();

            if (accnts.Count == 1)
            {
                Console.WriteLine(">>> Account found: " + accnts[0].FullName);
            }
            else
            {
                Console.WriteLine(">>> Account not found.");
            }
        }


        static async Task ShowAccounts()
        {
            List<IAccount> accnts = await g_app.GetAccounts().ToListAsync();

            if (accnts.Count > 0)
            {
                for (int i=0; i<accnts.Count; i++)
                {
                    Console.WriteLine(">>> " + (i+1) + ": " + accnts[i].FullName);
                }
            }
            else
            {
                Console.WriteLine(">>> No accounuts were found.");
            }
        }


        static async Task DeleteAccount(string email)
        {
            List<IAccount> accnts = await g_app.GetAccounts().Where(acct => acct.Email == email).ToListAsync();

            if (accnts.Count == 1)
            {
                Console.WriteLine(">>> Account deleted: " + accnts[0].FullName);

                await accnts[0].DeleteAsync();
            }
            else
            {
                Console.WriteLine(">>> Account not found.");
            }
        }
    }
}