# nopCommerce Api SampleApplication

In this sample project we have demonstrated how clients can authorize and obtain **Access/Refresh tokens** that can be used to access the [nopCommerce Web API](https://github.com/SevenSpikes/nopCommerce/tree/Web-Api-3.70), which is available as a plugin and provides a **RESTful API** to various resources in nopCommerce i.e Customers, Categories, Products, Shopping Cart Items, Orders.

[nopCommerce Web API](https://github.com/SevenSpikes/nopCommerce/tree/Web-Api-3.70) plugin support **OAuth 2.0 Authorization Code grant type**. This grant type is suitable for server applications that want to have access to the Web API resources.

The standard Authorization Code Grant flow is described in [Section 4.1](http://tools.ietf.org/html/rfc6749#section-4.1) of the The OAuth 2.0 Authorization Framework.

Here is how the Authorization Code Grant Flow works in nopCommerce Web API.

![Authorization Code Grant Flow Diagram](https://github.com/SevenSpikes/nopCommerce-Api-Authorization/blob/master/diagram.jpg "Authorization Code Grant Flow Diagram")

1. The client application starts the flow by redirecting the user agent to the nopcommerce Web API authorization endpoint. Sending the **client id** and **redirect url** to which the API should send the response.
2. The nopCommmerce authorization endpoint **redirects** the user agent back to the **redirect url** with an authorization code. Optionally you can add a **state** parameter and a **scope** parameter.
3. The client application requests an **access token** from the nopCommerce token issuance endpoint by presenting the **authorization code** that has just received along with the **client id**, **client secret** and the **redirect url**.
4. The nopCommerce token endpoint returns an **access token** and a **refresh token**. The refresh token can be used to request additional access tokens when they expired.
5. The client application uses the **access token** to authenticate to the Web API every time it makes request to access its resources.



