shader_type canvas_item;

// Cielo diurno de Marte para un ColorRect.
// Transparente de noche (se ven las estrellas de atrás), se va "encendiendo" al amanecer
// siguiendo una senoide, llega a verde pálido al mediodía y se vuelve a ocultar al atardecer.
// Mientras sale el sol, el tono pasa de un crepúsculo azulado/ámbar a verde pálido.
//
// `hora` la actualiza CieloMarciano.cs desde GameManager.SegundoMarciano.

uniform float hora : hint_range(0.0, 24.0) = 12.0;

group_uniforms Ciclo;
// Horas marcianas entre las que hay luz de día. La senoide va de 0 (amanecer) a 1 (mediodía) a 0 (atardecer).
uniform float hora_amanecer : hint_range(0.0, 12.0, 0.1) = 6.0;
uniform float hora_atardecer : hint_range(12.0, 24.0, 0.1) = 18.0;
// Opacidad máxima del cielo al mediodía (1.0 tapa por completo las estrellas).
uniform float alfa_maximo : hint_range(0.0, 1.0, 0.01) = 1.0;

group_uniforms Colores;
uniform vec4 color_zenit_dia : source_color = vec4(0.62, 0.77, 0.58, 1.0);
uniform vec4 color_horizonte_dia : source_color = vec4(0.82, 0.90, 0.71, 1.0);
uniform vec4 color_zenit_alba : source_color = vec4(0.20, 0.28, 0.43, 1.0);
uniform vec4 color_horizonte_alba : source_color = vec4(0.93, 0.64, 0.46, 1.0);

group_uniforms Transicion;
// Qué tan alto debe estar el sol (0-1) para que el cielo termine de pasar de tonos de alba a tonos de día.
uniform float duracion_alba : hint_range(0.1, 1.0, 0.01) = 0.55;
// Posición vertical del horizonte en pantalla (0 = arriba, 1 = abajo). El degradado vertical llega al color de horizonte ahí.
uniform float altura_horizonte : hint_range(0.1, 1.0, 0.01) = 0.65;

void fragment() {
	// 0 al amanecer, 1 al atardecer; fuera de ese rango se queda en los extremos.
	float duracion_dia = max(hora_atardecer - hora_amanecer, 0.001);
	float x = clamp((hora - hora_amanecer) / duracion_dia, 0.0, 1.0);

	// Altura del sol: senoide 0 -> 1 -> 0. Es también la opacidad del cielo.
	float sol = sin(PI * x);

	// El zenit llega antes al tono de día; el horizonte conserva más tiempo el brillo cálido del alba.
	float mezcla_zenit = smoothstep(0.0, duracion_alba, sol);
	float mezcla_horizonte = smoothstep(0.0, min(duracion_alba * 1.6, 1.0), sol);

	vec3 zenit = mix(color_zenit_alba.rgb, color_zenit_dia.rgb, mezcla_zenit);
	vec3 horizonte = mix(color_horizonte_alba.rgb, color_horizonte_dia.rgb, mezcla_horizonte);

	// Degradado vertical en espacio de pantalla (no depende del tamaño del ColorRect).
	float g = smoothstep(0.0, 1.0, clamp(SCREEN_UV.y / altura_horizonte, 0.0, 1.0));

	COLOR = vec4(mix(zenit, horizonte, g), sol * alfa_maximo);
}
