#version 330

in vec2 fragTexCoord;
in vec4 fragColor;

uniform sampler2D texture0;
uniform vec4 colDiffuse;

uniform vec2 playerScreenPos;
uniform float lightRadius;

out vec4 finalColor;

void main()
{
    vec4 texelColor = texture(texture0, fragTexCoord);

    float dist = distance(gl_FragCoord.xy, playerScreenPos);
    float fade = clamp(dist / lightRadius, 0.0, 0.9); // 0 = luz, 1 = escuro total


    vec3 darkened = texelColor.rgb * (1.0 - fade); // escurece a cor em vez de só a transparência

    finalColor = vec4(darkened, texelColor.a) * fragColor * colDiffuse;
}