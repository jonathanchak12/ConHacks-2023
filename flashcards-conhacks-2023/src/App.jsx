import { useState } from "react";
import "./App.css";
import Flashcard from "./Flashcard";
import reactLogo from "./assets/react.svg";
import viteLogo from "/vite.svg";

function App() {
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
      <div>
        <a href="https://vitejs.dev" target="_blank" rel="noreferrer">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank" rel="noreferrer">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>Flashcards - ConHacks 2023</h1>

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
            <div className="modal-header">New Flashcard</div>
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

export default App;
