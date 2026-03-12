let s = require('net').Socket();
s.connect(70, 'gopher.floodgap.com');
s.write('');

s.on('data', function(d){
    console.log(d.toString());
});

s.end();