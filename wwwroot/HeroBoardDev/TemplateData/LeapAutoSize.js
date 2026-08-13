(function()
{
    const unityContainer = document.querySelector('#unity-container');
    unityContainer.style.position = 'absolute';
    unityContainer.style.width = '100%';
    unityContainer.style.height = '100%';

    const unityCanvas = document.querySelector('#unity-canvas');
    unityCanvas.style.position = 'absolute';

    const initialDimensions = {width: parseInt(unityCanvas.getAttribute("width"), 10), height: parseInt(unityCanvas.getAttribute("height"), 10)};
	//console.log("> Dimensions : ", initialDimensions);	

    const changeDimensions = () => { setTimeout(setDimensions, 10); }
    const setDimensions = () =>
	{
        var winW = parseInt(window.getComputedStyle(unityContainer).width, 10);
        var winH = parseInt(window.getComputedStyle(unityContainer).height, 10);
		//console.log("> win : (" + winW + ", " + winH + ")");
		
        var scale = Math.min(winW / initialDimensions.width, winH / initialDimensions.height);
		//console.log("> scale : (" + scale + ")");

        var fitW = Math.round(initialDimensions.width * scale * 100) / 100;
        var fitH = Math.round(initialDimensions.height * scale * 100) / 100;
		//console.log("> fit : (" + fitW + ", " + fitH + ")");

        unityCanvas.style.width = fitW + 'px';
        unityCanvas.style.height = fitH + 'px';
    }
	
    window.addEventListener('resize', changeDimensions, false);
    changeDimensions();
})();
