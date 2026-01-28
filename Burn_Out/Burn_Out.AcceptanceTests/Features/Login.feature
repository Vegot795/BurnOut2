Feature: Login
  In order to access protected areas
  As a registered user
  I want to be able to sign in

  Scenario: Successful login with valid credentials
    Given the application is running
    And a user with email "client@example.com" and password "Pass!23" exists
    When I attempt to sign in with email "client@example.com" and password "Pass!23"
    Then the response should be a redirect to "/"