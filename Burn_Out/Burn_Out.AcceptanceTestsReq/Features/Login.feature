Feature: Login
  In order to access protected areas
  As a registered user
  I want to be able to sign in

  Scenario: Successful login with valid credentials
    Given the application is running
    And a user with email "test@example.com" and password "Password123!" exists
    When I attempt to sign in with email "test@example.com" and password "Password123!"
    Then the response should be a redirect to "/"