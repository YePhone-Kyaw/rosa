export default function RosaLogo() {
  return (
    <div className="flex items-center gap-2">
      <svg
        width="32"
        height="40"
        viewBox="0 0 56 68"
        xmlns="http://www.w3.org/2000/svg"
      >
        <circle cx="28" cy="28" r="24" fill="#E11D48" opacity="0.12" />
        <circle cx="28" cy="28" r="16" fill="#E11D48" opacity="0.2" />
        <path
          d="M28 12 C20 12, 10 20, 10 28 C10 38, 20 44, 28 44 C36 44, 46 38, 46 28 C46 20, 36 12, 28 12 Z"
          fill="none"
          stroke="#E11D48"
          strokeWidth="2"
        />
        <path
          d="M28 16 C24 18, 18 24, 20 30 C22 36, 28 38, 28 38"
          fill="none"
          stroke="#E11D48"
          strokeWidth="1.5"
          opacity="0.5"
        />
        <path
          d="M28 16 C32 18, 38 24, 36 30 C34 36, 28 38, 28 38"
          fill="none"
          stroke="#E11D48"
          strokeWidth="1.5"
          opacity="0.5"
        />
        <path
          d="M28 44 L28 56"
          stroke="#E11D48"
          strokeWidth="2"
          strokeLinecap="round"
        />
        <path
          d="M28 50 C24 46, 20 46, 18 48"
          fill="none"
          stroke="#E11D48"
          strokeWidth="1.5"
          strokeLinecap="round"
        />
      </svg>
      <span className="text-2xl font-bold text-rose-600">Rosa</span>
    </div>
  );
}
