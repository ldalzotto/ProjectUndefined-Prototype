using System;
using UnityEngine;

namespace AnimatorPlayable
{
    public static class GradiantBandDirectionalInterpolator
    {
        public static void SampleWeightsPolar(Vector2 sample_point, Vector2[] Points, ref float[] Weights)
        {
            const float kDirScale = 2.0f;
            float total_weight = 0f;
            float sample_mag = sample_point.magnitude;
            for (int i = 0; i < Points.Length; i++)
            {
                Vector2 point_i = Points[i];
                float point_mag_i = point_i.magnitude;
                float weight = 1f;

                for (int j = 0; j < Points.Length; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }

                    Vector2 point_j = Points[j];
                    float point_mag_j = point_j.magnitude;
                    float ij_avg_mag = (point_mag_i + point_mag_j) * 0.5f;

                    // Calc angle and mag for i -> sample
                    float mag_is = (sample_mag - point_mag_i) / ij_avg_mag;
                    float angle_is = signedAngle(point_i, sample_point);

                    // Calc angle and mag for i -> j
                    float mag_ij = (point_mag_j - point_mag_i) / ij_avg_mag;
                    float angle_ij = signedAngle(point_i, point_j);

                    // Calc vec for i -> sample
                    Vector2 vec_is = new Vector2(mag_is, angle_is * kDirScale);

                    // Calc vec for i -> j
                    Vector2 vec_ij = new Vector2(mag_ij, angle_ij * kDirScale);

                    // Calc weight
                    float lensq_ij = Vector2.Dot(vec_ij, vec_ij);
                    float new_weight = Vector2.Dot(vec_is, vec_ij) / lensq_ij;
                    new_weight = 1.0f - new_weight;
                    new_weight = Mathf.Clamp(new_weight, 0.0f, 1.0f);

                    weight = Math.Min(new_weight, weight);
                }

                Weights[i] = weight;

                total_weight += weight;
            }

            for (int i = 0; i < Points.Length; ++i)
            {
                Weights[i] = Weights[i] / total_weight;
            }
        }

        public static float signedAngle(Vector2 a, Vector2 b)
        {
            return Vector2.SignedAngle(a, b);
        }
    }
}


/*
 #define POINT_COUNT 8

const float kPi         = 3.14159;

void sampleWeightsCartesian(vec2 sample_point, vec2 points[POINT_COUNT], out float weights[POINT_COUNT] )
{
    float total_weight = 0.0;
    
    for( int i = 0; i < POINT_COUNT; ++i )
    {
        // Calc vec i -> sample
        vec2    point_i = points[i];
        vec2    vec_is  = sample_point - point_i;
        
        float   weight  = 1.0;
        
        for( int j = 0; j < POINT_COUNT; ++j )
        {
            if( j == i ) 
                continue;
            
            // Calc vec i -> j
            vec2    point_j     = points[j];            
            vec2    vec_ij      = point_j - point_i;      
            
            // Calc Weight
            float lensq_ij      = dot( vec_ij, vec_ij );
            float new_weight    = dot( vec_is, vec_ij ) / lensq_ij;
            new_weight          = 1.0 - new_weight;
            new_weight          = clamp(new_weight, 0.0, 1.0 );
            
            weight              = min(weight, new_weight);
        }
       
        weights[i]          = weight;
        total_weight        += weight;
    }
    
    for( int i = 0; i < POINT_COUNT; ++i )
    {
        weights[i] = weights[i] / total_weight;
    }
}

float signedAngle(vec2 a, vec2 b)
{
    return atan( a.x*b.y - a.y*b.x, a.x*b.x + a.y*b.y );
}

void sampleWeightsPolar(vec2 sample_point, vec2 points[POINT_COUNT], out float weights[POINT_COUNT] )
{
    
    const float kDirScale   = 2.0;
    
    float   total_weight    = 0.0;
    
    float   sample_mag      = length( sample_point );
    
    for( int i = 0; i < POINT_COUNT; ++i )
    {      
        vec2    point_i     = points[i];
        float   point_mag_i = length(point_i);
        
        float   weight      = 1.0;
        
        for( int j = 0; j < POINT_COUNT; ++j )
        {
            if( j == i ) 
                continue;
            
            vec2    point_j         = points[j];
            float   point_mag_j     = length( point_j );
            
            float   ij_avg_mag      = (point_mag_j + point_mag_i) * 0.5;
            
            // Calc angle and mag for i -> sample
            float   mag_is          = (sample_mag - point_mag_i) / ij_avg_mag;
            float   angle_is		= signedAngle(point_i, sample_point);
            
            // Calc angle and mag for i -> j
            float   mag_ij          = (point_mag_j - point_mag_i) / ij_avg_mag;
            float   angle_ij		= signedAngle(point_i, point_j);
            
            // Calc vec for i -> sample
            vec2    vec_is;
            vec_is.x                = mag_is;
            vec_is.y                = angle_is * kDirScale;
            
            // Calc vec for i -> j
            vec2    vec_ij;
            vec_ij.x                = mag_ij;
            vec_ij.y                = angle_ij * kDirScale;
            
            // Calc weight
         	float lensq_ij      = dot( vec_ij, vec_ij );
            float new_weight    = dot( vec_is, vec_ij ) / lensq_ij;
            new_weight          = 1.0-new_weight;
            new_weight          = clamp( new_weight, 0.0, 1.0 );
            
            weight              = min( new_weight, weight );
        }
        
        weights[i]          = weight;
        
        total_weight        += weight;
    }
    
    for( int i = 0; i < POINT_COUNT; ++i )
    {
		weights[i] = weights[i] / total_weight;
    }
}


void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    float scale = 3.0;
    
    vec2 aspect = vec2( iResolution.x / iResolution.y, 1.0 );
    
    vec2 uv = fragCoord.xy / iResolution.xy;
    uv -= 0.5;
    uv *= scale;
    uv *= aspect;
    
    vec2 mouse = iMouse.xy / iResolution.xy;
    mouse -= 0.5;
    mouse *= scale;
    mouse *= aspect;
    
    vec2 mouse2 = (iResolution.xy - iMouse.xy) / iResolution.xy;
    mouse2 -= 0.5;
    mouse2 *= scale;
    mouse2 *= aspect;
    
    vec2 points[POINT_COUNT];
    points[0] = mouse;
    points[7] = mouse2 ;
    points[1] = vec2( 1.0, 0.0 );
    points[2] = vec2( 0.0, 1.0 );
    points[3] = vec2(-1.0, 0.0 );
    points[4] = vec2( 0.0,-1.0 );
    points[5] = vec2( 0.0, 0.0 );
        
    float weights_polar[POINT_COUNT];
    sampleWeightsPolar(uv, points, weights_polar);
    
    float weights_linear[POINT_COUNT];
    sampleWeightsCartesian(uv, points, weights_linear);
    
  
    vec3 total_value   = vec3( weights_linear[0], weights_polar[7], 0.0 );
    total_value         = pow( total_value, vec3(1.0 / 2.2) ); // Linearize so we can see the full range
    
    fragColor.xyz       = vec3( total_value );
    
    for( int i = 0; i < POINT_COUNT; ++i )
    {
        float point_dot = ceil( pow( clamp(1.0-length(points[i] - uv), 0.0, 1.0), 1500.0) );
        fragColor.xyz   = mix(fragColor.xyz, vec3(1.0, 0.0, 1.0), point_dot);
    }
}
 * 
 */