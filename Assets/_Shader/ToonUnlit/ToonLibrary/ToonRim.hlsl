#ifndef TOON_RIM
#define TOON_RIM

half3 ToonRim(half3 worldNormal, half3 worldViewDirection, half3 lightColor, half lightAttenuation) {
    
     half fresnelOrientationPower = pow(abs((1.0 - saturate(dot(normalize(worldNormal), normalize(worldViewDirection)))) * lightAttenuation), _RimPower);
     half fresnel = saturate(step(_RimOffset, fresnelOrientationPower));
     return _RimColor * lightColor * fresnel;
}

#endif