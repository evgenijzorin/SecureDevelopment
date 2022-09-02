using jwtSample;

Console.WriteLine("Enter user name");
string userName = Console.ReadLine();
Console.WriteLine("Enter user password");
string pasword = Console.ReadLine();

UserService userService = new UserService();
string token = userService.Authenticate(userName, pasword);
Console.WriteLine(token);
Console.ReadKey(true);

