namespace Classes
{
    public static class Shaders
    {
        public static string VS = @"
#version 330 core

layout(location=0) in vec3 aPos;
layout(location=1) in vec3 aNormal;
layout(location=6) in vec2 aTexCoord; 
layout(location=2) in mat4 instanceModel;

uniform mat4 uView;
uniform mat4 uProj;

out vec3 vNormal;
out vec3 vWorld;
out vec2 vTexCoord;

void main()
{
    vec4 world = instanceModel * vec4(aPos, 1.0);
    vWorld = world.xyz;

    mat3 n = mat3(transpose(inverse(instanceModel)));
    vNormal = normalize(n * aNormal);

    vTexCoord = aTexCoord; // NEW: Pass the UVs through
    
    gl_Position = uProj * uView * world;
}";

        public static string FS = @"
#version 330 core

const float PI = 3.14159265359;

float DistributionGGX(vec3 N, vec3 H, float roughness)
{
    float a = roughness * roughness;
    float a2 = a * a;

    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;

    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return a2 / max(denom, 0.0001);
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = roughness + 1.0;
    float k = (r * r) / 8.0;

    return NdotV / (NdotV * (1.0 - k) + k);
}

float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);

    float ggxV = GeometrySchlickGGX(NdotV, roughness);
    float ggxL = GeometrySchlickGGX(NdotL, roughness);

    return ggxV * ggxL;
}

vec3 FresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

in vec3 vNormal;
in vec3 vWorld;
in vec2 vTexCoord;

uniform vec3 uCameraPos;

//uniform float uMetallic;
//uniform float uRoughness;

uniform sampler2D albedoTex;
uniform sampler2D mrTex;

out vec4 FragColor;

void main()
{
    vec3 lightPos = vec3(0, 9, 12);

    vec3 N = normalize(vNormal);
    vec3 V = normalize(uCameraPos - vWorld);
    vec3 L = normalize(lightPos - vWorld);
    vec3 H = normalize(V + L);

    float NdotL = max(dot(N, L), 0.0);
    float NdotV = max(dot(N, V), 0.0);
    float NdotH = max(dot(N, H), 0.0);
    float VdotH = max(dot(V, H), 0.0);

    // --- textures ---
    vec3 albedo = texture(albedoTex, vTexCoord).rgb;
    vec3 mr = texture(mrTex, vTexCoord).rgb;

    float metallic  = mr.b;
    float roughness = mr.g;

    // --- F0 (base reflectance) ---
    vec3 F0 = vec3(0.04);
    F0 = mix(F0, albedo, metallic);

    // --- BRDF terms ---
    float D = DistributionGGX(N, H, roughness);
    float G = GeometrySmith(N, V, L, roughness);
    vec3 F = FresnelSchlick(VdotH, F0);

    // --- specular ---
    vec3 numerator = D * G * F;
    float denom = max(4.0 * NdotV * NdotL, 0.001);
    vec3 specular = numerator / denom;

    // energy conservation (correct glTF form)
    vec3 kS = F;
    vec3 kD = (vec3(1.0) - kS) * (1.0 - metallic);
    kD = max(kD, vec3(0.0));

    vec3 diffuse = kD * albedo / PI;
    vec3 ambient = vec3(0.03) * albedo;
    vec3 color = ambient + (diffuse + specular) * NdotL;

    FragColor = vec4(color, 1.0);
}";
    }
}