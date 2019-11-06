# StegCrytoWannaBe
A simple application created for classes at university, it hides given text information (encrypted via AES-256) into loaded BMP image
on its least significant bits via xor'ing 2 source bits with 3 destination bits.
Effectively making the change to at most 1 bit in the source.
Math looks like this:
```
x1 == a1 ^ a3 and x2 == a2 ^ a3 => no change
x1 != a1 ^ a3 and x2 == a2 ^ a3 => ~a1
x1 == a1 ^ a3 and x2 != a2 ^ a3 => ~a2
x1 != a1 ^ a3 and x2 != a2 ^ a3 => ~a3
```
## Installation
```
$git clone https://github.com/extramask93/steganography
```
Or if You want to build from source make sure to install at least Visual Studio 15 with .net framework v4.6.1

## Usage
Simply load via Load button include Your message in the 'secret' text box and provide alternative salt for AES algorithm
(You will be needing it for decoding). Click embed, and result will appear below original image.
To reverse the process remeber to provide salt and load the image to upper left corner.
