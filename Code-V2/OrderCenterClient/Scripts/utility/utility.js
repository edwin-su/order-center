define(['jquery'], function ($) {
    var self = {};

    function showLoading () {
        $(".loading").show();
        return $(".loading").show();
    }
    function hideLoading () {
        $(".loading").hide();
        return $(".loading").hide();
    }

    self.baseAjax = function (parameters, noError) {
        var deferred = $.Deferred();
        $.when(showLoading())
        .done(function () {
            $.ajax(parameters)
            .done(function (data) {
                hideLoading();
                deferred.resolve(data);
            })
            .fail(function (xhr) {
                hideLoading();
                if (!noError) {
                    if (xhr.responseText) {
                        if (!!JSON.parse(xhr.responseText).ExceptionMessage) {
                            alert(JSON.parse(xhr.responseText).ExceptionMessage);
                        } else {
                            if (JSON.parse(xhr.responseText).Message == "Session Timeout") {
                                alert('会话超时，请重新登录');
                                location.href = "/";
                            } else {
                                alert(JSON.parse(xhr.responseText).Message);
                            }
                        }
                    } else {
                        alert("操作失败，请重试");
                    }
                }

                deferred.reject(xhr.responseText);
            });
        });
        return deferred;
    }

    self.isIOS = function () {
        return !!navigator.userAgent.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/);
    }
    self.isMobileDevice = function () {
        return !!navigator.userAgent.match(/(phone|pad|pod|iPhone|iPod|ios|iPad|Android|Mobile|BlackBerry|Windows Phone)/i);
    }
    self.isPC = function () {
        return !self.isMobileDevice();
    }
    self.isiPhone = function () {
        return self.isIOS() && self.isMobileDevice();
    }
    self.isAndroid = function () {
        return clientScript.IsMobileDevice() && (!clientScript.IsIOS());
    }
    self.isIpad = function () {
        return !!navigator.userAgent.match(/iPad/);
    }

    Date.prototype.format = function (fmt) {
        var o = {
            "M+": this.getMonth() + 1,                 //月份 
            "d+": this.getDate(),                    //日 
            "h+": this.getHours(),                   //小时 
            "m+": this.getMinutes(),                 //分 
            "s+": this.getSeconds(),                 //秒 
            "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
            "S": this.getMilliseconds()             //毫秒 
        };
        if (/(y+)/.test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        }
        for (var k in o) {
            if (new RegExp("(" + k + ")").test(fmt)) {
                fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
            }
        }
        return fmt;
    }  

    return self;
});