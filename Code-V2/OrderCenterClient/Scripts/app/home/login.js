define(['ko', 'utility', 'jquery', 'app'], function (ko, utility, $, app) {
    var self = {};

    self.userName = ko.observable("admin");

    self.password = ko.observable("abc123_");

    self.login = function () {
        utility.baseAjax({
            url: '/Home/Login',
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({ "userName": self.userName(), "password": self.password() })
        }).done(function (data) {
            app.permissions(data);
            app.goTo("/home");
        });
    }

    return self;
});