import Link from "next/link";
import { useEffect, useState } from "react";
import { slides } from "../../constants";

export default function HeroSlideshow() {
  const [current, setCurrent] = useState(0);

  useEffect(() => {
    const interval = setInterval(() => {
      setCurrent((prev) => (prev + 1) % slides.length);
    }, 3000);
    return () => clearInterval(interval);
  }, []);

  return (
    <div className="relative rounded-2xl overflow-hidden mb-12">
      <div
        className="relative h-64 flex items-center transition-all duration-500"
      >
        {slides.map((slide, index) => (
          <div
            key={index}
            className={`absolute inset-0 flex items-center p-12 bg-linear-to-r ${slide.bgColor} transition-opacity duration-700 ${
              index === current
                ? "opacity-100"
                : "opacity-0 pointer-events-none"
            }`}
          >
            <div className="px-10">
              <h1 className="text-4xl font-bold text-white mb-4">
                {slide.title}
              </h1>
              <p className="text-lg text-white/80 mb-6">{slide.subtitle}</p>
              <Link
                href={slide.buttonLink}
                className="bg-white text-gray-900 px-6 py-3 rounded-lg font-medium hover:bg-gray-100 transition"
              >
                {slide.buttonText}
              </Link>
            </div>
          </div>
        ))}
      </div>

      <div className="absolute bottom-4 left-1/2 -translate-x-1/2 flex gap-2">
        {slides.map((_, index) => (
          <button
            key={index}
            onClick={() => setCurrent(index)}
            className={`w-2 h-2 rounded-full transition ${
              index === current ? "bg-white w-6" : "bg-white/50"
            }`}
          />
        ))}
      </div>

      <button
        onClick={() =>
          setCurrent((prev) => (prev - 1 + slides.length) % slides.length)
        }
        className="absolute left-4 top-1/2 -translate-y-1/2 bg-white/20 hover:bg-white/40 text-white w-10 h-10 rounded-full flex items-center justify-center transition"
      >
        ‹
      </button>
      <button
        onClick={() => setCurrent((prev) => (prev + 1) % slides.length)}
        className="absolute right-4 top-1/2 -translate-y-1/2 bg-white/20 hover:bg-white/40 text-white w-10 h-10 rounded-full flex items-center justify-center transition"
      >
        ›
      </button>
    </div>
  );
}
