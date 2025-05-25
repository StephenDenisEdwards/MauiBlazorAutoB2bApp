# Markdown File

No problem—let’s break down Point 1 into more detail:

---

## 1. Sign into the Azure portal

1. **What is the Azure portal?**

   * It’s a website ([https://portal.azure.com](https://portal.azure.com)) where you manage anything in Azure: create resources, view usage, configure settings, etc. Think of it like the “dashboard” or control panel for your Azure account.

2. **Why do you need a Global Administrator (GA) account?**

   * Every Azure Active Directory (AD) has roles that control who can do what.
   * The **Global Administrator** role is the highest-privilege role in an Azure AD tenant—it can create new tenants, add or remove users, reset passwords, and more.
   * To create a **new B2C tenant**, you must already be signed in to an Azure AD with GA rights, because you’re effectively spinning up an entirely new AD directory.

3. **How to check if you’re a Global Admin**

   * After signing in at [https://portal.azure.com](https://portal.azure.com), click the menu (≡) in the top left and choose **Azure Active Directory**.
   * In the left-hand menu under **Manage**, pick **Roles and administrators**.
   * Look for “Global administrator” and click it—you’ll see a list of accounts in your directory that hold that role.
   * If your account is listed there, you’re all set.

4. **What if you’re not a Global Admin?**

   * You’ll need to ask whoever *is* the Global Admin in your current Azure AD (often your IT or Cloud team) to either:

     * Give your account the Global Admin role temporarily, **or**
     * Create the B2C tenant for you.

5. **Signing in**

   * Go to [https://portal.azure.com](https://portal.azure.com) in your browser.
   * Enter the email address that’s tied to your Azure AD (for example, your work account).
   * Enter your password (and, if required, any multi-factor authentication code).
   * Once logged in, you’ll land on the Azure Dashboard—which shows tiles for resources you’ve pinned, usage charts, quick-start guides, etc.

---

**TL;DR**: Point 1 just means “open your browser, go to the Azure portal, and log in with an account that has the top-level Global Administrator role in your existing Azure AD.” Once you’re in with GA rights, you’ll have permission to create a brand-new B2C tenant.



# Create

Here are the **bare-bones steps** to spin up an Azure AD B2C tenant (no fluff, just the high-level actions).  Pick whichever approach fits you—portal or CLI/REST.

---

## A) Portal (UI) approach

1. **Sign in**
   Go to [https://portal.azure.com](https://portal.azure.com) and log in with a **Global Administrator** account in *any* existing Azure AD tenant.

2. **Create the B2C tenant**

   * Click **Create a resource** (the “+ New” tile in the Home or left-rail).
   * In the Marketplace search box type **Azure Active Directory B2C** and select **Azure Active Directory B2C** → **Create**.
   * Choose **Create a new Azure AD B2C Tenant**, fill in your Organization name, initial domain (e.g. `contosob2c.onmicrosoft.com`), country/region—then **Review + Create** → **Create**.

3. **Link it for billing**

   * Once deployment finishes, open the new B2C tenant blade.
   * Under **Manage** click **Subscriptions** → **+ Link subscription**, pick which Azure subscription should pay for this tenant → **Link**.

4. **Switch into your new B2C directory**

   * Use the “Directories + subscriptions” picker in the top bar to change your **Directory** context to your new B2C tenant.
   * Only once you’re switched over will you see the **Azure AD B2C** entry under **All services**.

5. **Define your identity journeys**

   * In the B2C blade go to **User flows (policies)** → **+ New user flow** to create Sign-up/Sign-in, Password-reset, Profile-edit flows.

6. **Register your apps**

   * Still in the B2C tenant, go to **App registrations** → **+ New registration**, set the name and Redirect URI of your web/mobile app—grab the **Application (client) ID** and **Tenant ID** for your code.

---

## B) CLI/REST (programmatic) approach

1. **Log in with Azure CLI**

   ```bash
   az login
   # ensure you're in the right subscription
   az account set --subscription <your-subscription-id>
   ```

2. **(If needed) Register the AAD provider**

   ```bash
   az provider register --namespace Microsoft.AzureActiveDirectory
   ```

3. **Create the B2C tenant**

   ```bash
   az rest \
     --method PUT \
     --uri "https://management.azure.com/subscriptions/<subId>/resourceGroups/<rg>/providers/Microsoft.AzureActiveDirectory/b2cDirectories/<tenantName>?api-version=2023-01-18-preview" \
     --body '{
       "location": "westeurope",
       "properties": { "displayName": "Contoso B2C Tenant" }
     }'
   ```

4. **Link it to your subscription**

   ```bash
   az rest \
     --method POST \
     --uri "https://management.azure.com/providers/Microsoft.AzureActiveDirectory/b2cDirectories/<tenantName>/linkSubscription?api-version=2023-01-18-preview" \
     --body '{"subscriptionId":"<subId>"}'
   ```

5. **Switch directory context**
   In the portal’s “Directories + subscriptions” picker, choose your newly created B2C tenant.

6. **Automate user flows & app registrations**
   You can further script out creation of user flows and app registrations via ARM/Bicep or the Microsoft Graph APIs once your B2C tenant exists.

---

That’s it—six core steps, whichever path you choose. Let me know which one you’ll try or where you get stuck!





# 1️⃣ Login and select your subscription
az login
az account set --subscription "YOUR_SUBSCRIPTION_ID"    (37628991-ee2f-44e1-9608-d6e2e5ede592)

# 2️⃣ (Idempotent) Register the AAD provider so you can create B2C directories
az provider register --namespace Microsoft.AzureActiveDirectory

# 3️⃣ Create a resource group for the B2C directory object
az group create --name "MyMauiB2CResourceGroup" --location "westeurope"

# 4️⃣ Create the B2C tenant (replace placeholders)
- az rest --method PUT --uri "https://management.azure.com/subscriptions/YOUR_SUBSCRIPTION_ID/resourceGroups/MyB2CResourceGroup/providers/Microsoft.AzureActiveDirectory/b2cDirectories/CONTOSOB2C?api-version=2023-01-18-preview" --body "{\"location\":\"westeurope\",\"properties\":{\"displayName\":\"Contoso B2C Tenant\"}}"

az rest --method PUT --uri "https://management.azure.com/subscriptions/37628991-ee2f-44e1-9608-d6e2e5ede592/resourceGroups/MyMauiB2CResourceGroup/providers/Microsoft.AzureActiveDirectory/b2cDirectories/mauib2c?api-version=2023-01-18-preview" --body "{\"location\":\"westeurope\",\"properties\":{\"displayName\":\"Maui B2C Tenant\"}}"



# 5️⃣ Link the new B2C tenant for billing
az rest --method POST --uri "https://management.azure.com/providers/Microsoft.AzureActiveDirectory/b2cDirectories/CONTOSOB2C/linkSubscription?api-version=2023-01-18-preview" --body "{\"subscriptionId\":\"YOUR_SUBSCRIPTION_ID\"}"

# 6️⃣ (Optional) Verify it exists
az rest --method GET --uri "https://management.azure.com/subscriptions/YOUR_SUBSCRIPTION_ID/resourceGroups/MyB2CResourceGroup/providers/Microsoft.AzureActiveDirectory/b2cDirectories?api-version=2023-01-18-preview" -o table



flowchart TD
  %% Interactive Login Flow
  subgraph Interactive_Login ["Interactive Login Flow"]
    direction LR
    U[User taps **Sign In**] --> MSAL_INT[MSAL.NET AcquireTokenInteractive]
    MSAL_INT --> SYSB[System Browser<br/>(Chrome Custom Tab / ASWebAuth)]
    SYSB --> B2C[B2C Sign-In/Sign-Up UI]
    B2C --> TOKENS[ID Token + Access Token<br/>(+ Refresh Token)]
    TOKENS --> APP1[MAUI App<br/>updates Blazor UI]
  end

  %% Silent Token Refresh Flow
  subgraph Silent_Refresh ["Silent Token Refresh Flow"]
    direction LR
    APP2[App Startup<br/>or Token Needed] --> MSAL_SILENT[MSAL.NET AcquireTokenSilent]
    MSAL_SILENT --> CACHE[MSAL Token Cache<br/>(Access + Refresh)]
    CACHE -->|if expired| B2C_REFRESH[B2C Token Endpoint]
    B2C_REFRESH --> NEWTOKENS[New Access Token<br/>(+ renewed Refresh Token)]
    NEWTOKENS --> APP3[MAUI App continues]
  end

  %% API Call Flow
  subgraph API_Call ["API Call Flow"]
    direction LR
    APP4[MAUI App<br/>HTTP call w/ bearer token] --> MID[JwtBearer Middleware<br/>(Microsoft.Identity.Web)]
    MID --> DISCOVERY[Validate via B2C Discovery<br/>(iss, aud, exp, scopes)]
    DISCOVERY --> AUTHZ[Authorize(Attribute/Policy)]
    AUTHZ --> RESOURCE[Protected Resource<br/>in Web API]
  end

  %% Arrange subgraphs vertically
  Interactive_Login --- Silent_Refresh
  Silent_Refresh --- API_Call
