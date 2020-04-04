# **Single Sign On (SSO)**

MLW SSO works over OAuth 2.0 and OpenId Connect Protocol. It uses RedHat Identity Server : KeyCloak to authenticate and authorize Users.

## How User logs in with Identity Server.
<img width="100%" src="https://github.com/SoftwareAG/MLW/blob/master/docs/design/SSO/SSO%20-%20OAuth2.0%20OpenIdConnect.png"/>

## How MLW can work with other SSO system.
To enable SSO feature in MLW for other SSO system, MLW environment should be updated. <br/>
For docker, .env file and for standalone MLW appsettings.json file needs to configure with below attributes.
*   AuthorizationEndpoint  (https://accounts.mlw.ai/auth/realms/master)
*   TokenEndpoint (https://accounts.mlw.ai/auth/realms/master/protocol/openid-connect/token)
*   UserInformationEndpoint   (https://accounts.mlw.ai/auth/realms/master/protocol/openid-connect/userinfo)
*   ClientId    (zmod-app-dev)
*   ClientSecret  (24324324242342-2342-23432-43242)
*   ResponseType  (code id_token)
*   IsSecuredHTTP (false)
*   Realm   (master)
<img width="100%" src="https://github.com/SoftwareAG/MLW/blob/master/docs/design/SSO/MLW%20with%20other%20SSO%20System.png"/>
