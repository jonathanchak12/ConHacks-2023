import { useState } from "react";
import "./Flashcard.css";

function Flashcard({ term, definition }) {
  const [isFlipped, setIsFlipped] = useState(false);

  const handleCardClick = () => {
    setIsFlipped(!isFlipped);
  };

  return (
    <div
      className={`flashcard ${isFlipped ? "flipped" : ""}`}
      onClick={handleCardClick}
    >
      <div className="flashcard-content flashcard-front">{term}</div>
      <div className="flashcard-content flashcard-back">{definition}</div>
    </div>
  );
}

export default Flashcard;
