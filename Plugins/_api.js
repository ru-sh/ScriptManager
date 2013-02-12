// API description
api = {
    console: {
        onOutputReceived: function(fn) {},
        write: function (cmd) { }
    },

    script: {
        executeNextCommand: function() {}
    },

    trace:{
        write: function (str) { },
        writeLine: function (str) { }
    }
};