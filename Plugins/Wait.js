/// <reference path="_api.js"/>

(function(_, wait) {
    wait.seconds = function(s) {
        setTimeout(_.script.executeNextCommand, s * 1000);
    };

    wait.string = function(s) {
        _.script.onOutputReceived(function(str) {
            if (str.indexOf(s) != -1) {
                _.script.executeNextCommand();
            }
        });
    };
})(api, wait = wait == null ? {} : wait);