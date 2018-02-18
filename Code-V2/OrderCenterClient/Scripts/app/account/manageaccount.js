define(['ko', 'utility', 'jquery'], function (ko, utility, $) {
    var self = {};

    self.operatorList = ko.observableArray([]);
    self.defaultOperator = ko.observable();
    self.hasError = ko.observable(false);
    self.errorMsg = ko.observable("");

    function resetDefaultOperator() {
        var operator = {};
        operator.DisplayName = ko.observable("");
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
            url: '/Home/GetOperatorList',
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
            url: '/Home/UpdateOperator',
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
            url: '/Home/DeleteOperator',
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
            url: '/Home/ChangeOperatorActiveStatus',
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({ "operatorInfo": ko.mapping.toJS(data) })
        }).done(function (result) {
            if (!!result && result.ChangeStatusSuccessfully) {
                var list = [];
                $.each(self.operatorList(), function (key, value) {
                    if (value.OperatorId() === data.OperatorId()) {
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
        if (!!data && !!data.OperatorName() && !!data.Password()) {
            resetError();
            if (data.OperatorName().length > 2 && data.Password().length > 2) {
                utility.baseAjax({
                    url: '/Home/CreateNewDefaultOperator',
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json",
                    data: JSON.stringify({ "operatorInfo": ko.mapping.toJS(data) })
                }).done(function (result) {
                    if (!!result && result.CreateNewSuccessfully) {
                        var list = self.operatorList();
                        data.DisplayName(data.OperatorName());
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