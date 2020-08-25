<h1>Marquee Examples Libvlcharp</h1>
<h2>Enable</h2>
<code><pre>
// 1 to enable marquee displaying. Marquee can be seen. 
// 0 to disable marquee displaying. Marquee will not visible.
MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Enable, 1); 
</pre></code>
<h2>#Size</h2>
<code><pre>
//Basically Font size in pixels. default_value=0
//bigger int value means bigger marquee text will be displayed
MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Size, 32); 
</pre></code>
<h2>#Position</h2>
<img src="https://wiki.videolan.org/images/Marq_demonstration_-_VLC_3.0.6_Linux.png">
<code><pre>
// Marquee position: 0=center, 1=left, 2=right, 4=top, 8=bottom 
// You can also use combinations of these values, eg 6 = top-right. default_value=0
MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Position, 8); 
</pre></code>
<h2>#Y and X axis</h2>
<code><pre>
//            ^
//            | Y axis
//            | 
//            |50x      
//            |--------*                          
//            |        | 
//            |        | 90y
//            |        | 
//            |        | 
// <----------|------------------>
//            |                   X axis
//            |
// symbol "*" shows your text. It will be appears like in diagram if you do set axis like below
MediaPlayer.SetMarqueeInt(VideoMarqueeOption.X, 50); // X offset, from the left screen edge. default_value=0
MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Y, 90); // Y offset, down from the top. default_value=0
</pre></code>
<h2>#Opacity</h2>
<code><pre>
// Opacity means rate of NOT transparency
// More opacity more text will have tough sides
// Value should be b/w 0--255]
// Opacity (inverse of transparency) of overlaid text. 
// 0 = transparent, 255 = totally opaque. 
// default value: 255
MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Opacity, 100); 
</pre></code>
<h2>#Text</h2>
<code><pre>
// Text to be displayed as marquee text
// Here "my text" is example
MediaPlayer.SetMarqueeString(VideoMarqueeOption.Text, "my text"); //Text to display
</pre></code>
<h2>Color</h2>
<code><pre>
// Set color in decimal NOT HEXADECIMAL.
// I was first using hex like 0xffffff. But it should be pure decimal.
// Go here to convert hex to decimal or to generate color code: https://www.mathsisfun.com/hexadecimal-decimal-colors.html 
// default_value=0xffffff means white color. 
// Example 16711680 for red. 
MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Color, 16711680); 
</pre></code>
More Advance options here https://wiki.videolan.org/Documentation:Modules/marq/
