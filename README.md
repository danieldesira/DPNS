# DPNS - A web Push Notification Server

This is a web Push Notification Platform that sends notifications to services owned by the major browser developers in order to 
distribute to subscribed end-users. The current repository consists of a simple REST API documented at `{host}/scalar`. 

Moreover, the [DPNS.http](https://github.com/danieldesira/DPNS/blob/master/DPNS/DPNS.http) contains examples that may be tested 
through a suitable client such as the [_REST Client_ VS Code extension](https://marketplace.visualstudio.com/items?itemName=humao.rest-client).

However, _Visual Studio 2026_ is recommended for development.

In order to run the migrations, use the following:
`dotnet ef database update --context DpnsDbContext`
