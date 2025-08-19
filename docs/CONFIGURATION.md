## Configuration: OAuth & Email

To use external logins and email features, **you must create your own OAuth apps and SendGrid account, then add your credentials using User Secrets** (recommended for local development).

### 1. Create OAuth Apps
- **Google:**
  - Go to [Google Cloud Console](https://console.developers.google.com/), create an OAuth app, and get your Client ID and Client Secret.
- **GitHub:**
  - Go to [GitHub Developer Settings](https://github.com/settings/developers), create an OAuth app, and get your Client ID and Client Secret.

### 2. Create a SendGrid Account
- Sign up at [SendGrid](https://sendgrid.com/), create an API key, and verify your sender email.

### 3. Add Credentials via User Secrets
From your project directory, run:
```sh
dotnet user-secrets set "Authentication:Google:ClientId" "<your-google-client-id>"
dotnet user-secrets set "Authentication:Google:ClientSecret" "<your-google-client-secret>"
dotnet user-secrets set "Authentication:GitHub:ClientId" "<your-github-client-id>"
dotnet user-secrets set "Authentication:GitHub:ClientSecret" "<your-github-client-secret>"
dotnet user-secrets set "SendGrid:ApiKey" "<your-sendgrid-api-key>"
dotnet user-secrets set "SendGrid:FromEmail" "<your-verified-sender-email>"
dotnet user-secrets set "SendGrid:FromName" "SnipEx"
```

**Example structure (with placeholders):**
```json
"Authentication": {
  "Google": {
    "ClientId": "",
    "ClientSecret": ""
  },
  "GitHub": {
    "ClientId": "",
    "ClientSecret": ""
  }
},
"SendGrid": {
  "ApiKey": "",
  "FromEmail": "",
  "FromName": ""
}
```

> **Note:**
> - Never commit your real API keys or secrets to source control.
> - Each user must set up their own credentials to use these features.

## How to Use the New Features

- **External Login:**
  - On the login page, click “Continue with Google” or “Continue with GitHub.”
- **Forgot Password:**
  - Click “Forgot password?” on the login page, enter your email, and follow the instructions in the email you receive.
