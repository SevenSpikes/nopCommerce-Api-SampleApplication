# nopCommerce Api Sample Client Application

In this sample project we have demonstrated how clients can authorize and obtain **Access/Refresh tokens** that can be used to access the [nopCommerce Web API](https://github.com/SevenSpikes/api-plugin-for-nopcommerce), which is available as a plugin and provides a **RESTful API** to various resources in nopCommerce i.e Customers, Categories, Products, Shopping Cart Items, Orders.

[nopCommerce Web API](https://github.com/SevenSpikes/api-plugin-for-nopcommerce) plugin support **OAuth 2.0 Authorization Code grant type**. This grant type is suitable for server applications that want to have access to the Web API resources.

The standard Authorization Code Grant flow is described in [Section 4.1](http://tools.ietf.org/html/rfc6749#section-4.1) of the The OAuth 2.0 Authorization Framework.

Here is how the Authorization Code Grant Flow works.
---------------------------------------------------------------------------

![Authorization Code Grant Flow Diagram](https://github.com/SevenSpikes/nopCommerce-Api-Authorization/blob/master/diagram.jpg "Authorization Code Grant Flow Diagram")

1. The client application starts the flow by redirecting the user agent to the nopcommerce Web API authorization endpoint. Sending the **client id** and **redirect url** to which the API should send the response.

2. The nopCommmerce authorization endpoint **redirects** the user agent back to the **redirect url** with an authorization code. Optionally you can add a **state** parameter and a **scope** parameter.

3. The client application requests an **access token** from the nopCommerce token issuance endpoint by presenting the **authorization code** that has just received along with the **client id**, **client secret** and the **redirect url**.

4. The nopCommerce token endpoint returns an **access token** and a **refresh token**. The refresh token can be used to request additional access tokens when they expired.

5. The client application uses the **access token** to authenticate to the Web API every time it makes request to access its resources.

How to use the client application.
---------------------------------------------------------------------------

1. Open the solution (NopCommerce.Api.SampleApplication.sln) and run it.

2. It will start a website that will prompt you for **Server url**, **Client Id** and **Client Secret**, which you need manually populate a bit later. The **Redirect url** will be automatically populated with the url of your sample website so you don't have to change it.

3. Run your nopCommerce website with the Api plugin installed. Copy the nopCommerce website url and use it for the **Server url**.

4. Create a new Api client from the Api plugin administration and give it some name i.e **Sample Client**.

5. The Client Id and Client Secret will be automatically generated for you so you just need to copy and use them into the Sample Application.

6. Now copy the Redirect Url from the Sample Application and set it to the client's Callback Url and Save the client.

7. Now when you click on Get Access Token in the Sample Application you will be able to obtain an Access Token that you can use to access resources in your nopCommerce store.

8. We have created a Get Customers link that will use the Access Token to retrieve the customers from the nopCommerce store.



