import "./Navbar.css";

function Navbar() {
  return (
    <nav className="nav">
      <a href="/" className="site-title">
        FlashyCon
      </a>
      <ul>
        <li>
          <a href="/courses">Courses</a>
        </li>
        <li>
          <a href="/unit">Unit</a>
        </li>
        <li>
          <a href="/qa">Q/A</a>
        </li>
      </ul>
    </nav>
  );
}

export default Navbar;
