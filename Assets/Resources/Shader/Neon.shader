Shader "Custom/NeonShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1) // Base color
        _MainTex ("Albedo (RGB)", 2D) = "white" {} // Albedo texture
        _Glossiness ("Smoothness", Range(0, 1)) = 0.5 // Smoothness
        _Metallic ("Metallic", Range(0, 1)) = 0.0 // Metallic
        _EmissionColor ("Emission Color", Color) = (0, 1, 1, 1) // Neon Emission color (usually bright)
        _EmissionIntensity ("Emission Intensity", Range(0, 5)) = 1.0 // Glow strength
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _EmissionColor;
        half _EmissionIntensity;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            // Emission is added for a glowing effect
            fixed4 emission = _EmissionColor * _EmissionIntensity;
            o.Emission = emission.rgb;  // Emit the emission color based on intensity
        }
        ENDCG
    }
    FallBack "Diffuse"
}
