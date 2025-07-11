# TcgPlatformApi

A REST API for your TCG (Trading Card Game) client. You can also use the companion client: [TcgPlatformClient](#).

## Technologies
- .NET 8
- Entity Framework Core
- PostgreSQL
- JWT
- BCrypt
- xUnit
- Moq

## API Endpoints

### Avatar
- `POST /api/avatar/uploadavatar`
- `GET /api/avatar/getavatar`

### Booster
- `POST /api/booster/addboosters`
- `GET /api/booster/getallboosters`
- `POST /api/booster/removeboosters`

### Card
- `POST /api/card/addcards`
- `GET /api/card/getallcards`
- `POST /api/card/removecards`

### Deck
- `POST /api/deck/addorupdatedecks`
- `POST /api/deck/removedecks`
- `GET /api/deck/getdeck`
- `GET /api/deck/getalldecks`

### Email
- `POST /api/email/email`

### Profile
- `GET /api/profile/getprofile`
- `POST /api/profile/createprofile`
- `POST /api/profile/updateprofile`

### RegVerLog
- `POST /api/regverlog/register`
- `POST /api/regverlog/verify`
- `POST /api/regverlog/login`
- `POST /api/regverlog/resetpassword`
- `POST /api/regverlog/verifyresetpassword`

### Token
- `POST /api/token/refreshtoken`

## Installation & Running

1. Clone the repository:
   ```bash
   git clone https://github.com/CYHTF7/TcgPlatformApi.git
   ```   
2. Create user secrets:
   ```net
   dotnet user-secrets init
   ```
3. Go to secters and setup:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost; Port=5432; Database=your_db_name; Username=your_db_username; Password=your_db_password"
     },
     "SmtpSettings": {
       "Host": "smtp.your_email_provider.com",          
       "Port": 587,                                     
       "Username": "your_email@example.com",            
       "Password": "your_app_password",                 
       "EnableSsl": true
     },
     "JwtSettings": {
       "SecretKey": "your_jwt_secret_key",              
       "RefreshTokenSecret": "your_refresh_token_key",  
       "Issuer": "your_app_name_or_url",                
       "Audience": "your_client_app_name",              
       "ExpiresInHours": 2,                             
       "RefreshTokenExpiresInDays": 7                   
     }
   }
   ```
4. Create migration:
   ```net
   dotnet tool install --global dotnet-ef
   dotnet ef migrations add AddRegistrationFields
   dotnet ef database update
   ```
5. Run:
   ```net
   dotnet run --launch-profile "https"
   ```
