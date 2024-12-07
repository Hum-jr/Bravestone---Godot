shader_type spatial;
render_mode cull_disabled;

uniform vec3 color: source_color;
uniform vec3 color2: source_color;
uniform sampler2D noise;
uniform float noiseScale = 20.0;
uniform float voronoiScale = 5.0;
uniform float voronoiIntensity = 0.5;

varying vec3 worldPos;

// Voronoi noise function
float voronoi(vec2 x) {
    vec2 n = floor(x);
    vec2 f = fract(x);
    vec2 mg, mr;
    float md = 8.0;
    
    for(int j = -1; j <= 1; j++) {
        for(int i = -1; i <= 1; i++) {
            vec2 g = vec2(float(i), float(j));
            vec2 o = vec2(
                fract(sin(dot(n + g, vec2(12.9898, 78.233))) * 43758.5453),
                fract(cos(dot(n + g, vec2(12.9898, 78.233))) * 43758.5453)
            );
            
            vec2 r = g + o - f;
            float d = length(r);
            
            if(d < md) {
                md = d;
                mr = r;
                mg = g;
            }
        }
    }
    
    return md;
}

void vertex() {
    // Called for every vertex the material is visible on.
    worldPos = (MODEL_MATRIX * vec4(VERTEX,1.0)).xyz;
}

void fragment() {
    // Called for every pixel the material is visible on.
    
    // Original noise
    vec3 noiseLevel = texture(noise, worldPos.xz / noiseScale).rgb;
    
    // Voronoi noise
    float voronoiNoise = voronoi(worldPos.xz * voronoiScale);
    
    // Mix colors with both original noise and Voronoi noise
    vec3 mixedColor = mix(color, color2, UV.y) * 
                      mix(color, color2, noiseLevel.r) * 
                      (1.0 - voronoiNoise * voronoiIntensity);
    
    ALBEDO = mixedColor;
    
    if (!FRONT_FACING) {
        NORMAL = -NORMAL;
    }
}
//void light() {
    // Called for every pixel for every light affecting the material.
    // Uncomment to replace the default light processing function with this one.
//}