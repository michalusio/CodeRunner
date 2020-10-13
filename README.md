# CodeRunner
A simple code runner with WinForms front and a HTTP endpoint. Maybe I'll make it into a game?

### Using the endpoints

To use the Code runner, you have to register a user, log in using the credentials, and input some code.

#### To register:
> POST https://<ip>:8443/account/register
  
The body of the message has to be a JSON like this:
```
{
  "Username": "admin",
  "Email": "admin@admin.com",
  "Password": "admin"
}
```
The server will respond with 201 Created if the account is succesfully created.
  
#### To log in:
> POST https://<ip>:8443/account/login
  
The body of the message has to be a JSON like this:
```
{
  "Email": "admin@admin.com",
  "Password": "admin"
}
```
The server will respond with 200 OK and send a session cookie if the user has successfully logged in.

#### To send some code to the server:
> PUT https://<ip>:8443/code
  
The body of this message is the code that should be run, like this:
```
public class Program
{
    public static void Main(PlayerData player)
    {
        var dir = (Random.Integer() % 5) - 2;
        player.Robots[0].Move((Direction) dir);
    }
}
```
The server will respond with 201 Created if the player has never before put any code. Otherwise, it will respond with 200 OK.
Your request has to have the correct session cookie.

#### To get code back from the server:
> GET https://<ip>:8443/code
  
The server will respond with the code of the currently logged in player (200 OK) or with a 404 Not Found, if the user hasn't send any code yet.
Your request has to have the correct session cookie.
