var slideSpeed : float = -25f;
var repeat : boolean = true;
var scrollArea : Vector2 = Vector2(0, 800);

function OnGUI()
{
	if(repeat)
	{
		if((GetComponent.<GUITexture>().pixelInset.x + GetComponent.<GUITexture>().texture.width) < scrollArea.x)
			GetComponent.<GUITexture>().pixelInset.x = scrollArea.y;
		else if((GetComponent.<GUITexture>().pixelInset.x + GetComponent.<GUITexture>().texture.width) > scrollArea.y)
			GetComponent.<GUITexture>().pixelInset.x = scrollArea.x;
			
		GUI.depth = 1;
		GUI.DrawTexture
		(
			Rect
			(
				(slideSpeed > 0 ? GetComponent.<GUITexture>().pixelInset.x - GetComponent.<GUITexture>().texture.width : GetComponent.<GUITexture>().pixelInset.x + GetComponent.<GUITexture>().texture.width), 
				0,//guiTexture.pixelInset.y, 
				GetComponent.<GUITexture>().texture.width, 
				GetComponent.<GUITexture>().texture.height
			), 
			GetComponent.<GUITexture>().texture
		);
	}
	
	GetComponent.<GUITexture>().pixelInset.x += slideSpeed * Time.deltaTime;
}