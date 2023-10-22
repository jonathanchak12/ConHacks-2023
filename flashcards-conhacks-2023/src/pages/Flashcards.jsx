import { useEffect, useRef, useState } from "react";
import Flashcard from "../FlashcardComponent";

function Flashcards() {
  const [flashcards, setFlashcards] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [term, setTerm] = useState("");
  const [definition, setDefinition] = useState("");

  const modalRef = useRef(null);

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (modalRef.current && !modalRef.current.contains(event.target)) {
        setShowModal(false);
      }
    };

    const handleEscKey = (event) => {
      if (event.key === "Escape" || event.key === "Esc") {
        setShowModal(false);
      }
    };

    if (showModal) {
      document.addEventListener("mousedown", handleClickOutside);
      document.addEventListener("keydown", handleEscKey);
    } else {
      document.removeEventListener("mousedown", handleClickOutside);
      document.removeEventListener("keydown", handleEscKey);
    }

    // Cleanup on component unmount
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
      document.removeEventListener("keydown", handleEscKey);
    };
  }, [showModal]);

  const handleNewFlashcard = () => {
    if (term && definition) {
      setFlashcards([...flashcards, { term, definition }]);
      setTerm("");
      setDefinition("");
      setShowModal(false);
    }
  };

  return (
    <>
      <div>
        {flashcards.map((flashcard, index) => (
          <Flashcard
            key={index}
            term={flashcard.term}
            definition={flashcard.definition}
          />
        ))}

        <button onClick={() => setShowModal(true)}>Add Flashcard</button>

        {showModal && (
          <div className="modal open" ref={modalRef}>
            <div className="modal-header">Create New Flashcard</div>
            <input
              className="modal-input"
              placeholder="Term"
              value={term}
              onChange={(e) => setTerm(e.target.value)}
            />
            <input
              className="modal-input"
              placeholder="Definition"
              value={definition}
              onChange={(e) => setDefinition(e.target.value)}
            />
            <button className="modal-button" onClick={handleNewFlashcard}>
              Add
            </button>
          </div>
        )}
      </div>
    </>
  );
}

export default Flashcards;
