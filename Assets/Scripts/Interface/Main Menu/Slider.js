var slides : Texture2D[];
var slideInterval = 4f;

private var slideIndex = 0;

InvokeRepeating("UpdateSlide", 0, slideInterval);

function UpdateSlide() 
{
	guiTexture.texture = slides[slideIndex % slides.length];
	slideIndex++;
}