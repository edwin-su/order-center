define(['ko', 'utility', 'jquery', 'app'], function (ko, utility, $, app) {
    var self = {};

    self.AllowToViewOrder = ko.observable(false);
    self.AllowToChangeOrder = ko.observable(false);
    self.AllowToModifyMeal = ko.observable(false);
    self.AllowToViewSales = ko.observable(false);
    self.AllowToModifyAccount = ko.observable(false);
    self.AllowToModifyPaymentMethod = ko.observable(false);

    self.init = function () {
        utility.baseAjax({
            url: '/Home/GetOperatorPermission',
            type: "GET",
            dataType: "json",
        }).done(function (data) {
            if (!!data && !!data.OperatorPermission) {
                self.AllowToViewOrder(data.OperatorPermission.AllowToViewOrder);
                self.AllowToChangeOrder(data.OperatorPermission.AllowToChangeOrder);
                self.AllowToModifyMeal(data.OperatorPermission.AllowToModifyMeal);
                self.AllowToViewSales(data.OperatorPermission.AllowToViewSales);
                self.AllowToModifyAccount(data.OperatorPermission.AllowToModifyAccount);
                self.AllowToModifyPaymentMethod(data.OperatorPermission.AllowToModifyPaymentMethod);
            }
        });
    }

    return self;
});