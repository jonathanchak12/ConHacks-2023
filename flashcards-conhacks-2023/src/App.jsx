import { useState } from "react";
import "./App.css";
import Flashcard from "./Flashcard";
import Navbar from "./Navbar";
import flashyConLogo from "./assets/FlashyCon Logo.png";

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
        <Navbar></Navbar>
      </div>
      <div>
        {" "}
        <a href="/" target="_blank">
          <img
            className="logo"
            src={flashyConLogo}
            alt="FlashyCon Logo"
            width={100}
            height={100}
          />
        </a>
      </div>
      <h1>FlashyCon - ConHacks 2023</h1>

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

export default App;
