var MyPlugin = {
    Hello: function()
    {
        window.alert("Hello, world!");
    },
    HelloString: function(str)
    {
        window.alert(Pointer_stringify(str));
    },
    PrintFloatArray: function(array, size)
    {
        for(var i=0;i<size;i++)
            console.log(HEAPF32[(array>>2)+size]);
    },
    AddNumbers: function(x,y)
    {
        return x + y;
    },
    StringReturnValueFunction: function()
    {
        var returnStr = "bla";
        var buffer = _malloc(lengthBytesUTF8(returnStr) + 1);
        writeStringToMemory(returnStr, buffer);
        return buffer;
    },

    URLParams: function( name )
    {
	var urlParams;
	var match,
        pl     = /\+/g,  // Regex for replacing addition symbol with a space
        search = /([^&=]+)=?([^&]*)/g,
        decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
        query  = window.location.search.substring(1);

	urlParams = {};
    while (match = search.exec(query)){
	console.log( decode(match[1]) );
	console.log( decode(match[2]) );
       urlParams[decode(match[1])] = decode(match[2]);
    }

	name = Pointer_stringify(name);
	console.log( name + " is the question" );
	var returnStr = urlParams[ decode(name) ];
	console.log( returnStr + " is the answer" );

        var buffer = _malloc(lengthBytesUTF8(returnStr) + 1);
        writeStringToMemory(returnStr, buffer);
        return buffer;
    },



    BindWebGLTexture: function(texture)
    {
        GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[texture]);
    }
};

mergeInto(LibraryManager.library, MyPlugin);
