FROM adorsys/keycloak-config-cli:latest-26.0.0

COPY realm.json /config/realm.json
COPY users-nonprod.json /config/users-nonprod.json
COPY entrypoint.sh /entrypoint.sh

ENV KEYCLOAK_URL=http://keycloak:8080
ENV IMPORT_MANAGED=full
ENV IMPORT_VAR_SUBSTITUTION_ENABLED=true
ENV IMPORT_FILES_LOCATIONS=/config/realm.json
ENV KC_INCLUDE_TEST_USERS=false

USER root
RUN chmod +x /entrypoint.sh
USER 1000

ENTRYPOINT ["/entrypoint.sh"]
