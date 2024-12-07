shader_type spatial;

uniform float foam_speed : hint_range(0.0, 10.0) = 2.0;
uniform float foam_scale : hint_range(0.0, 20.0) = 10.0;
uniform float foam_width : hint_range(0.0, 1.0) = 0.2;
uniform vec4 water_color : source_color = vec4(0.2, 0.4, 0.8, 0.8);
uniform vec4 foam_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);

void vertex() {
    // Optional vertex manipulation for wave-like movement
    VERTEX.y += sin(TIME * 0.5 + VERTEX.x * 0.5) * 0.1;
}

void fragment() {
    // Create vertical noise for foam stripes
    float noise = fract(UV.x * foam_scale + TIME * -foam_speed);
    
    // Create sharp-edged foam stripes
    float foam = step(1.0 - foam_width, noise);
    
    // Blend water and foam colors
    vec3 final_color = mix(water_color.rgb, foam_color.rgb, foam * foam_color.a);
    
    // Set albedo and alpha
    ALBEDO = final_color;
    ALPHA = water_color.a + foam * (1.0 - water_color.a);
}