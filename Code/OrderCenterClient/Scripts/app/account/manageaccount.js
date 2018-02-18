define(['ko', 'utility', 'jquery'], function (ko, utility, $) {
    var self = {};

    self.operatorList = ko.observableArray([]);
    self.defaultOperator = ko.observable();
    self.hasError = ko.observable(false);
    self.errorMsg = ko.observable("");

    function resetDefaultOperator() {
        var operator = {};
        operator.UserName = ko.observable("");
        operator.Password = ko.observable("");
        operator.OperatorName = ko.observable();
        operator.AllowToViewOrder = ko.observable(false);
        operator.AllowToChangeOrder = ko.observable(false);
        operator.AllowToModifyMeal = ko.observable(false);
        operator.AllowToViewSales = ko.observable(false);
        operator.AllowToModifyAccount = ko.observable(false);
        operator.AllowToModifyPaymentMethod = ko.observable(false);
        operator.IsActive = ko.observable();
        self.defaultOperator(operator);
    }

    function init() {
        resetDefaultOperator();
        resetError();
        utility.baseAjax({
            url: '/Account/GetOperatorList',
            type: "GET",
            dataType: "json",
        }).done(function (data) {
            var list = [];
            $.each(data.OperatorList, function (key, value) {
                var operator = ko.mapping.fromJS(value);
                operator.Password = ko.observable("");
                list.push(operator);
            })

            self.operatorList(list);
        });

    }

    self.updateAccount = function (data) {
        utility.baseAjax({
            url: '/Account/UpdateOperator',
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({ "operatorInfo": ko.mapping.toJS(data) })
        }).done(function (result) {
            if (!!result && result.UpdateOperator) {
                alert("更新成功");
            }
        });
    }

    self.deleteAccount = function (data) {
        utility.baseAjax({
            url: '/Account/DeleteOperator',
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({ "operatorInfo": ko.mapping.toJS(data) })
        }).done(function (result) {
            if (!!result && result.DeleteSuccessfully) {
                var list = [];
                $.each(self.operatorList(), function (key, value) {
                    if (value.OperatorId() != data.OperatorId()) {
                        list.push(value);
                    }
                })

                self.operatorList(list);
            }
        });
    }

    self.changeAccountActiveStatus = function (data) {
        utility.baseAjax({
            url: '/Account/ChangeAccountActiveStatus',
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({ "operatorInfo": ko.mapping.toJS(data) })
        }).done(function (result) {
            if (!!result && result.ChangeStatusSuccessfully) {
                var list = [];
                $.each(self.operatorList(), function (key, value) {
                    if (value.OperatorId() == data.OperatorId()) {
                        value.IsActive(result.OperatorStatus);
                    }
                    list.push(value);
                })

                self.operatorList(list);
            }
        });
    }

    var const_EmptyUsernameOrPasswordErrorMsg = '用户名或密码为空';
    var const_LengthUsernameOrPasswordErrorMsg = '用户名或密码长度小于3';
    var const_ExistedUsernameErrorMsg = "用户已存在";

    function resetError() {
        self.hasError(false);
        self.errorMsg("");
    }

    self.createNewDefaultOperator = function (data) {
        if (!!data && !!data.UserName() && !!data.Password()) {
            resetError();
            if (data.UserName().length > 2 && data.Password().length > 2) {
                utility.baseAjax({
                    url: '/Account/CreateNewDefaultOperator',
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json",
                    data: JSON.stringify({ "operatorInfo": ko.mapping.toJS(data) })
                }).done(function (result) {
                    if (!!result && result.CreateNewSuccessfully) {
                        var list = self.operatorList();
                        data.OperatorName(data.UserName());
                        data.Password("");
                        list.push(data);
                        self.operatorList(list);
                        resetDefaultOperator();
                    }
                })
                .fail(function(ex) {
                        self.hasError(true);
                        self.errorMsg(const_ExistedUsernameErrorMsg);
                });
            } else {
                self.hasError(true);
                self.errorMsg(const_LengthUsernameOrPasswordErrorMsg);
            }
            
        } else {
            self.hasError(true);
            self.errorMsg(const_EmptyUsernameOrPasswordErrorMsg);
        }
    }

    self.init = function () {
        init();
    }

    return self;
});