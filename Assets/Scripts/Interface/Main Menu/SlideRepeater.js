var slideSpeed : float = -25f;
var repeat : boolean = true;
var scrollArea : Vector2 = Vector2(0, 800);

function OnGUI()
{
	if(repeat)
	{
		if((guiTexture.pixelInset.x + guiTexture.texture.width) < scrollArea.x)
			guiTexture.pixelInset.x = scrollArea.y;
		else if((guiTexture.pixelInset.x + guiTexture.texture.width) > scrollArea.y)
			guiTexture.pixelInset.x = scrollArea.x;
			
		GUI.depth = 1;
		GUI.DrawTexture
		(
			Rect
			(
				(slideSpeed > 0 ? guiTexture.pixelInset.x - guiTexture.texture.width : guiTexture.pixelInset.x + guiTexture.texture.width), 
				0,//guiTexture.pixelInset.y, 
				guiTexture.texture.width, 
				guiTexture.texture.height
			), 
			guiTexture.texture
		);
	}
	
	guiTexture.pixelInset.x += slideSpeed * Time.deltaTime;
}