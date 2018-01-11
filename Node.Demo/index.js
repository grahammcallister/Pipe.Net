var net = require('net');

var PIPE_PATH = "\\\\.\\pipe\\Pipe.Server";

var client = net.connect(PIPE_PATH, function() {
    console.log('Client: on connection');
	client.on('data', (data) => { 
		console.log('Incoming ' + data.toString()); 
	});
});

const readline = require('readline');
readline.emitKeypressEvents(process.stdin);
//process.stdin.setRawMode(true);

process.stdin.on('keypress', (str, key) => {

console.log('Key pressed');
  
	var msg = 'Something ' + Date.now() + '\r\n';
  	client.write(msg);
	  //if(key.ctrl == true && key.name == 'c'){
    setTimeout(process.exit(), 3000);
	  //}
});

