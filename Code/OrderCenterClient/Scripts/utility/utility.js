define(['jquery'], function ($) {
    var self = {};

    self.baseAjax = function (parameters, noError) {
        var deferred = $.Deferred();
        $.ajax(parameters)
        .done(function (data) {
            deferred.resolve(data);
        })
        .fail(function (xhr) {
            if (!noError) {
                if (xhr.responseText) {
                    alert(JSON.parse(xhr.responseText).ExceptionMessage);
                } else {
                    alert("未知问题。。。。");
                }
            }

            deferred.reject(xhr.responseText);
        });

        return deferred;
    }

    return self;
});