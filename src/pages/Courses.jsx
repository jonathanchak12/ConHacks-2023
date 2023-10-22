import "./Courses.css";

function Button({ label, onClick }) {
  return (
    <button className="course-buttons" onClick={onClick}>
      {label}
    </button>
  );
}

function Courses() {
  return (
    <div className="courses-page">
      <h1>Courses</h1>
      <Button
        label="New"
        onClick={() => {
          /* Create New Course */
        }}
      />
      <Button
        label="Remove"
        onClick={() => {
          /* Select course to remove */
        }}
      />
    </div>
  );
}

export default Courses;
