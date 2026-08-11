#version 330

in vec2 TexCoords;
in vec4 fragColor;

uniform sampler2D texture0;

out vec4 finalColor;

void main()
{
    vec4 texColor = texture(texture0, TexCoords);

    finalColor = vec4(1.0, 1.0, 1.0, texColor.a) * fragColor;
}