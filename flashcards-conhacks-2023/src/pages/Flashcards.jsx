import { useState } from "react";
import Flashcard from "../Flashcard";
import Navbar from "../Navbar";

function Flashcards() {
  const [flashcards, setFlashcards] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [term, setTerm] = useState("");
  const [definition, setDefinition] = useState("");

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
      <Navbar></Navbar>
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
          <div className="modal open">
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
