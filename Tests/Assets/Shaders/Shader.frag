#version 410 core

const float ln2 = log(2);

in vec2 fpos;
out vec4 frag;

uniform vec3 cursorClr;
uniform vec2 vsize;
uniform float aspectRatio;
uniform mat4 rot;
uniform int maxIter;
uniform vec4 clrRatio;
uniform int julia_mode;
uniform bool crosshair;

uniform int fractalType; //0 mandelbrot, 1 julia
uniform dvec2 julia_c;
uniform dvec2 pos;
uniform double zoom;

struct CNum {
	double re, im;
};

CNum cadd(CNum a, CNum b) {
	return CNum(a.re + b.re, a.im + b.im);
}
CNum csub(CNum a, CNum b) {
	return CNum(a.re - b.re, a.im - b.im);
}
CNum cmul(CNum a, CNum b) {
	return CNum(a.re * b.re - a.im * b.im, a.re * b.im + a.im * b.re);
}
CNum csquare(CNum a) {
	return CNum(a.re * a.re - a.im * a.im, 2.0 * a.re * a.im);
}
CNum cdiv(CNum a, CNum b) {
	double den = b.re * b.re + b.im * b.im;
	if (den == 0) return CNum(0.0, 0.0);
	return CNum((a.re * b.re + a.im * b.im) / den, (a.im * b.re + a.re * b.im) / den);
}
double cmodsq(CNum a) {
	return a.re * a.re + a.im * a.im;
}
CNum crecpcl(CNum a) {
	double den = cmodsq(a);
	if (den == 0) return CNum(0.0, 0.0);
	return CNum(a.re / den, -a.im / den);
}

float iterJulia(CNum z) {
	CNum c = CNum(julia_c.x, julia_c.y);
	switch (julia_mode) {
		case 1:
			z = crecpcl(z);
		break;
		case 2:
			z = crecpcl(csquare(z));
		break;
		case 3:
			z = cmul(cadd(z, crecpcl(z)), crecpcl(z));
		break;
	}
	for (int i = 0; i < maxIter; i++) {
		z = csquare(z);	
		z = cadd(z, c);
		if (cmodsq(z) > 4) {
			z = csquare(z);
			z = cadd(z, c);
			z = csquare(z);
			z = cadd(z, c);
			float mod = float(cmodsq(z));
			if (mod <= 1) return 0;
			float mu = i - log(log(mod)) / ln2;
			if (mu < 0) return 0;
			return mu;
		}
	}
	return maxIter;
}

float iterMandelbrot(CNum c) {
	CNum z = CNum(0, 0);
	for (int i = 0; i < maxIter; i++) {
		z = csquare(z);
		z = cadd(z, c);
		if (cmodsq(z) > 4) {
			z = csquare(z);
			z = cadd(z, c);
			z = csquare(z);
			z = cadd(z, c);
			float mod = float(cmodsq(z));
			if (mod <= 1) return i;
			float mu = i - log(log(mod)) / ln2;
			return mu;
		}
	}
	return maxIter;
}

void main(void) {
	dvec2 z = fpos;
	bool line = false;
	float ch = 0.002;
	if ((abs(z.x) < ch || abs(z.y) < ch) && (z.x * z.x + z.y * z.y <= ch)) {
		line = true;
	}
	float viewRatio = vsize.x / vsize.y;
	z.y /= aspectRatio * viewRatio;
	double calczoom = zoom;
	calczoom *= viewRatio;
	z = (dvec4(z, 0, 1) * rot).xy;
	z.x *= calczoom;
	z.y *= calczoom;
	z += pos;

	float iter = 0;
	if (fractalType == 0) {
		iter = iterMandelbrot(CNum(z.x, z.y));
	} else if (fractalType == 1) {
		iter = iterJulia(CNum(z.x, z.y));
	}
	
	if (iter == maxIter) {
		frag = vec4(0, 0, 0, 1);
	} else {
		frag = vec4((-cos(clrRatio.x*iter)+1.0)/2.0, (-cos(clrRatio.y*iter)+1.0)/2.0, (-cos(clrRatio.z*iter)+1.0)/2.0, clrRatio.w);
	}

	if (line && crosshair) {
		frag.x = 1.0 - frag.x;
		frag.y = 1.0 - frag.y;
		frag.z = 1.0 - frag.z;
		frag.xyz *= cursorClr;
	}
}

