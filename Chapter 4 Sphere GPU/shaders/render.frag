#version 420 core

in vec2 TexCoords;

out vec4 color;

layout (binding=0) uniform sampler2D screenTexture;

void main()
{
	color = texture(screenTexture, TexCoords);
}
