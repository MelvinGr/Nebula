var slides : Texture2D[];
var slideInterval = 4f;

private var slideIndex = 0;

InvokeRepeating("UpdateSlide", 0, slideInterval);

function UpdateSlide() 
{
	GetComponent.<GUITexture>().texture = slides[slideIndex % slides.length];
	slideIndex++;
}