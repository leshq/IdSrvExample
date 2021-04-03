var config = {
    userStore: new Oidc.WebStorageStateStore({ store: window.localStorage }),
    authority: "https://localhost:5000",
    client_id: "client_id_js",
    redirect_uri: "https://localhost:50050/Home/SignIn",
    post_logout_redirect_uri: "https://localhost:50050/Home/Index",
    response_type: "code",
    scope: "openid IdSrvExample.scope IdSrvExample.assets"

}

var userManager = new Oidc.UserManager(config);

var signIn = function () {
    console.log("clicked signIn button");
    userManager.signinRedirect();
}

var signOut = function () {
    userManager.signoutRedirect();
};

userManager.getUser().then(user => {
    console.log("user:", user);
    if (user) {

        var date = new Date(0);
        date.setUTCSeconds(user.expires_at);
        console.log("token expiration time", date);

        axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;
    }
});

var callPrivateApi = function () {
    axios.get("https://localhost:5010/authtest/private").then(result => {
        console.log(result);
    });
}

var callAdminApi = function () {
    axios.get("https://localhost:5010/authtest/admin").then(result => {
        console.log(result);
    });
}

var refreshingToken = false;

axios.interceptors.response.use(
    function (response) {
        return response;
    },
    function (error) {
        console.log(error.response);

        var axiosConfig = error.response.config;

        // try to refresh access token in case of 401
        if (error.response.status === 401) {

            // if already in progress then skip
            if (!refreshingToken) {
                refreshingToken = true;

                return userManager.signinSilent().then(user => {
                    console.log("sign in silent result", user);
                    if (user) {

                        var date = new Date(0);
                        date.setUTCSeconds(user.expires_at);
                        console.log("new expiration time", date);

                        axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;
                        axiosConfig.headers["Authorization"] = "Bearer " + user.access_token;
                    }
                    refreshingToken = false;
                    // retrying original request but with a new token
                    return axios(axiosConfig);
                });
            }
        }


        return Promise.reject(error);
    });